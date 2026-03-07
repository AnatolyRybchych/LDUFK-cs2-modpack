
namespace CSSMngr;

public class CSSMngrMicroShDispatcherProvider
{
    private EventContext eventContext;
    public CSSMngrMicroShDispatcherProvider(EventContext ctx)
    {
        eventContext = ctx;
    }

    private string StartsWith(string input, string[] args) => args.Any((arg) => input.StartsWith(arg))
        ? "true" : "false";

    private string Echo(string input, string[] args)
    {
        return string.Join(" ", args);
    }

    private string Eval(string input, string[] args)
    {
        return new MicroSh().Compile(getDispatcher(), input)("", args);
    }

    public Func<string, MicroSh.Function> getDispatcher()
    {
        return (string functionName) =>
        {
            switch (functionName)
            {
                case "starts_with": return StartsWith;
                case "echo": return Echo;
                case "eval": return Eval;
                default:
                    Func<string> getter = eventContext.ToStringParam(functionName)
                        ?? throw new Exception($"Unexpected symbol '{functionName}'");
                    return (string input, string[] args) => getter();
            }
        };
    }
}