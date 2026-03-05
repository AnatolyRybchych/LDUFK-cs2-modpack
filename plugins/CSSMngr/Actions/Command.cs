
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CSSMngr.Actions;

public class Command: Action
{
    readonly string command;
    public Command(JsonElement parameters): base(parameters)
    {
        command = parameters.GetProperty("command").GetString()
            ?? throw new Exception("Missing required parameter 'command'");
    }

    override public void Execute(EventContext ctx)
    {
        Server.ExecuteCommand(command);
    }
}