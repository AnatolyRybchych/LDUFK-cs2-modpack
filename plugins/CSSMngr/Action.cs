
using System.Text.Json;

namespace CSSMngr;

public abstract class Action
{
    public Action(JsonElement _){}

    public abstract void Execute(EventContext ctx);
}