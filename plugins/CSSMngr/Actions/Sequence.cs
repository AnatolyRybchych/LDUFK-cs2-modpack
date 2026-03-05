
using System.Text.Json;
using CounterStrikeSharp.API;

namespace CSSMngr.Actions;

public class Sequence: Action
{
    List<Action> actions;
    public Sequence(IEnumerable<Action> actions) : base(new JsonElement())
    {
        this.actions = actions.ToList();
    }

    override public void Execute(EventContext ctx) => actions.ForEach(action => action.Execute(ctx));
}