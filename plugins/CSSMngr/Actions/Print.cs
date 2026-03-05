
using System.Text.Json;
using CounterStrikeSharp.API;

namespace CSSMngr.Actions;

public class Print: Action
{
    readonly string message;
    public Print(JsonElement parameters): base(parameters)
    {
        message = parameters.GetProperty("message").GetString()
            ?? throw new Exception("Missing required parameter 'message'");
    }

    override public void Execute(EventContext ctx)
    {
        Server.PrintToChatAll(message);
    }
}