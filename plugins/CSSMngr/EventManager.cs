
namespace CSSMngr;

public class EventManager<T> where T: EventContext, new()
{
    public T Ctx {get;} = new();
    public List<Action<T>> Handlers {get;} = [];

    public void Handle(Action<T> updateCtx)
    {
        updateCtx(Ctx);
        Handlers.ForEach((handler) => handler(Ctx));
    }
}