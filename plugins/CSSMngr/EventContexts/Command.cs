
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace CSSMngr.EventContexts;

public class Command: EventContext
{
    private CCSPlayerController? player;
    private CommandInfo? cmd;

    public void Update(CCSPlayerController? player, CommandInfo cmd)
    {
        this.player = player;
        this.cmd = cmd;
    }

    public override Func<bool>? BoolParam(string parameter) => parameter switch
    {
        "command.player.isadmin" => () => false,
        _ => base.BoolParam(parameter),
    };

    public override Func<string>? StringParam(string parameter) => parameter switch
    {
        "command" => () => cmd!.GetArg(0),
        _ => base.StringParam(parameter),
    };
}
