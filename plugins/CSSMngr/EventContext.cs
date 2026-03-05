
namespace CSSMngr;

public class EventContext
{
    public virtual Func<bool>? BoolParam(string parameter) => null;
    public virtual Func<string>? StringParam(string parameter) => null;

    public virtual Func<ulong>? UintParam(string parameter) => null;
    public virtual Func<long>? IntParam(string parameter) => null;

    public virtual Func<DateTime>? DateTimeParam(string parameter) => parameter switch
    {
        "date" => () => DateTime.Now,
        _ => null,
    };

    public Func<string>? ToStringParam(string parameter)
    {
        Func<string>? stringParam = StringParam(parameter);
        if (stringParam != null)
            return stringParam;

        Func<DateTime>? dateTimeParameter = DateTimeParam(parameter);
        if (dateTimeParameter != null)
            return () => dateTimeParameter().ToString();

        Func<long>? intParam = IntParam(parameter);
        if (intParam != null)
            return () => intParam().ToString();

        Func<ulong>? uintParam = UintParam(parameter);
        if (uintParam != null)
            return () => uintParam().ToString();

        Func<bool>? boolParam = BoolParam(parameter);
        if (boolParam != null)
            return () => boolParam().ToString();

        return null;
    }
}