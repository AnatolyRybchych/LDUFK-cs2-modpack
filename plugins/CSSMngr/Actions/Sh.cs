
using System.Text.Json;
using CounterStrikeSharp.API;

namespace CSSMngr.Actions;

public class Sh: Action
{
    readonly string command;
    public Sh(JsonElement parameters): base(parameters)
    {
        command = parameters.GetProperty("command").GetString()
            ?? throw new Exception("Missing required parameter 'command'");
    }

    override public void Execute(EventContext ctx)
    {
        var microShDispatcher = new CSSMngrMicroShDispatcherProvider(ctx).getDispatcher();
        try
        {
            Server.PrintToChatAll(new MicroSh().Compile(microShDispatcher, command)("", []));
        }
        catch (Exception e)
        {
            Server.PrintToChatAll($"{command}: {e.Message}");
        }
    }
}