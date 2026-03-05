
using System.Text.Json;

namespace CSSMngr;

public record ActionDescription(
    string eventName,
    Func<EventContext, Func<bool>> conditionChecker,
    Action action
);
