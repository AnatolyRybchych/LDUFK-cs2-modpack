
using CounterStrikeSharp.API.Core;

namespace CSSMngr.EventContexts;

public class PlayerDeath: EventContext
{
    private EventPlayerDeath? evt;
    private GameEventInfo? info;

    public void Update(EventPlayerDeath evt, GameEventInfo info)
    {
        this.evt = evt;
        this.info = info;
    }
}
