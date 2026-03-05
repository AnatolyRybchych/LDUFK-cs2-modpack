using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Events;
using CSSMngr.Actions;
using CSSMngr.EventContexts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CSSMngr;


public class CSSMngr : BasePlugin
{
    public override string ModuleName => "CSSMngr";
    public override string ModuleVersion => "0.0.0";
    public override string ModuleAuthor => "Anatolii Rybchych";
    public override string ModuleDescription => "CounterStrikeSharp management plugin";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("CSSMngr loaded.");

        try
        {
            var content = File.ReadAllText($"{ModuleDirectory}/commands.json");
            var json = JsonDocument.Parse(content);
            var commands = json.RootElement;
            foreach (var command in commands.EnumerateObject())
            {
                string? description = command.Value.GetString();
                AddCommand(command.Name, description ?? command.Name, OnCommand);

                if(description == null)
                    Logger.LogWarning($"The description is missing for the '{description}'");
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"commands.json: {e}");
        }

        IEnumerable<string> actionFiles = Utils.TryOr(
            () => Directory.EnumerateFiles($"{ModuleDirectory}/actions/", "*.json"),
            []);

        var listeners = new Dictionary<string, List<ActionDescription>>();
        foreach (var file in actionFiles)
        {
            try
            {
                var content = File.ReadAllText(file);
                var json = JsonDocument.Parse(content);
                ActionDescription description = GenerateActionDescription(json.RootElement);

                if (listeners.ContainsKey(description.eventName))
                    listeners[description.eventName].Append(description);
                else
                    listeners[description.eventName] = [description];
            }
            catch (Exception e)
            {
                Logger.LogError($"{file}: {e}");
            }
        }

        foreach (var listener in listeners)
        {
            try
            {
                EventContext eventContext = getEventContext(listener.Key);

                List<Action<EventContext>> listToRun = [];
                foreach (var actionDescription in listener.Value)
                {
                    Func<bool> checkCondition = actionDescription.conditionChecker(eventContext);
                    listToRun.Add(ctx =>
                    {
                        if (checkCondition())
                            actionDescription.action.Execute(ctx);
                    });
                }

                switch (listener.Key)
                {
                    case "map.loaded": mapLoaded.Handlers.AddRange(listToRun); break;
                    case "player.death": playerDeath.Handlers.AddRange(listToRun); break;
                    case "command": command.Handlers.AddRange(listToRun); break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{e}");
            }
        }

        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    EventContext getEventContext(string evt) => evt switch
    {
        "map.loaded" => mapLoaded.Ctx,
        "player.death" => playerDeath.Ctx,
        "command" => command.Ctx,
        _ => globalCotext
    };

    EventContext globalCotext = new();

    EventManager<EventContexts.Command> command = new();
    void OnCommand(CCSPlayerController? player, CommandInfo cmd)
    {
        command.Handle(ctx => ctx.Update(player, cmd));
    }

    EventManager<MapLoaded> mapLoaded = new();
    private void OnMapStart(string mapName)
    {
        mapLoaded.Handle((ctx) => ctx.Update(mapName));
    }

    EventManager<PlayerDeath> playerDeath = new();
    private HookResult OnPlayerDeath(EventPlayerDeath evt, GameEventInfo info)
    {
        playerDeath.Handle((ctx) => ctx.Update(evt, info));
        return HookResult.Continue;
    }

    private ActionDescription GenerateActionDescription(JsonElement json)
    {
        if (!json.TryGetProperty("do", out JsonElement actionProp))
            throw new Exception("The action does not define any action");

        List<JsonElement> actionElements;
        if (actionProp.ValueKind == JsonValueKind.Array)
            actionElements = actionProp.EnumerateArray().ToList();
        else if (actionProp.ValueKind == JsonValueKind.Object)
            actionElements = new List<JsonElement>{actionProp};
        else
            throw new Exception($"Unepected action type {actionProp.ValueKind}; expected array or object");

        List<Action> actions = actionElements.Select((json) =>
        {
            string actionName = json.GetProperty("action").GetString() ?? throw new Exception("Missing required string parameter 'action'");
            return actionName switch
            {
                "chat.print" => (Action)new Actions.Print(json.GetProperty("params")),
                "command" => (Action)new Actions.Command(json.GetProperty("params")),
                _ => throw new Exception($"The action '{actionName}' is not defined")
            };
        }).ToList();

        Action action = actions.Count == 1 ? actions[0] : new Sequence(actions);

        var evt = json.GetProperty("event").GetString()
            ?? throw new Exception("Missing required field 'event' in the action definition");

        Func<EventContext, Func<bool>> conditionChecker = (EventContext eventContext) =>
        {
            JsonElement conditionDescriptions;
            if (!json.TryGetProperty("if", out conditionDescriptions))
                return () => true;  
            
            List<Func<bool>> conditions= [];
            foreach (var condition in conditionDescriptions.EnumerateObject())
            {
                Func<string> propGetter = eventContext.ToStringParam(condition.Name)
                    ?? throw new Exception($"The '{condition.Name}' condtion cannot be met in '{evt}' event");

                conditions.Add(() => propGetter() == condition.Value.ToString());
            }

            return () => conditions.All((cnd) => cnd());
        };

        return new ActionDescription(
            eventName: evt,
            conditionChecker: conditionChecker,
            action: action);
    }
}