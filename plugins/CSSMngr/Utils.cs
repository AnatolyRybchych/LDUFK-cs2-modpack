
namespace CSSMngr;

class Utils
{
    public static T TryOr<T>(Func<T> func, T fallback)
    {
        try
        {
            return func();
        }
        catch
        {
            return fallback;
        }
    }
}