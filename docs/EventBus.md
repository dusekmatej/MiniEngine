# Event bus

`MiniEngine.Events.EventBus` lets parts of the engine and the game communicate
through events without knowing about each other. It is synchronous and has no
global state: create an instance and pass it to whoever needs it.

```csharp
using MiniEngine.Events;

public readonly record struct PlayerDamaged(int PlayerId, int Damage);

var events = new EventBus();

void OnPlayerDamaged(PlayerDamaged e)
{
    Console.WriteLine($"Player {e.PlayerId} took {e.Damage} damage");
}

events.Subscribe<PlayerDamaged>(OnPlayerDamaged);

events.Publish(new PlayerDamaged(1, 25));

events.Unsubscribe<PlayerDamaged>(OnPlayerDamaged);
```

Any type can be an event. A `readonly record struct` is a good default.

## Behavior

- **Matching:** listeners are matched by the exact event type. Publishing
  `PlayerDamaged` never calls listeners of another type, including base types
  or interfaces.
- **Order:** listeners run in the order they subscribed. Subscribing the same
  listener twice calls it twice.
- **No listeners:** publishing an event nobody listens to does nothing.
- **Unsubscribe:** removes one subscription of the listener (the last one) and
  returns `false` if the listener was not subscribed. To unsubscribe a lambda,
  keep it in a variable and pass the same instance.
- **Changes during publish:** a publish that is already running keeps the
  listeners it started with. A listener added during it runs from the next
  publish. A listener removed during it is still called in that publish if it
  had not run yet, and not afterwards. This matches how C# events behave.
- **Exceptions:** if a listener throws, the remaining listeners still run.
  After all of them have run, `Publish` throws one `AggregateException` with
  every listener exception in `InnerExceptions`, in listener order.

## Validation

Run the dependency-free console checks with:

```sh
dotnet run --project tests/Engine.Tests/Engine.Tests.csproj
```
