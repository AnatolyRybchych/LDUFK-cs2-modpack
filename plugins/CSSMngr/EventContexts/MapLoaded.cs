
namespace CSSMngr.EventContexts;

public class MapLoaded: EventContext
{
    private string mapName = "";

    public void Update(string mapName)
    {
        this.mapName = mapName;
    }

    public override Func<string>? StringParam(string parameter) => parameter switch
    {
        "map.loaded.name" => () => mapName,
        _ => base.StringParam(parameter),
    };
}
