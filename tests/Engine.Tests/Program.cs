using MiniEngine.Events;

{
    var events = new EventBus();
    var received = new List<PlayerDamaged>();

    void OnPlayerDamaged(PlayerDamaged e) => received.Add(e);

    events.Subscribe<PlayerDamaged>(OnPlayerDamaged);
    events.Publish(new PlayerDamaged(1, 25));
    Check(received.Count == 1 && received[0] == new PlayerDamaged(1, 25), "A subscribed listener should receive the event data.");

    Check(events.Unsubscribe<PlayerDamaged>(OnPlayerDamaged), "Unsubscribe should report a removed listener.");
    events.Publish(new PlayerDamaged(1, 10));
    Check(received.Count == 1, "An unsubscribed listener should not be called again.");
    Check(!events.Unsubscribe<PlayerDamaged>(OnPlayerDamaged), "Unsubscribing an unknown listener should return false.");
}

{
    var events = new EventBus();
    var calls = new List<string>();

    events.Subscribe<PlayerDamaged>(_ => calls.Add("first"));
    events.Subscribe<PlayerDamaged>(_ => calls.Add("second"));
    events.Publish(new PlayerDamaged(1, 5));

    Check(calls.SequenceEqual(["first", "second"]), "All listeners should run in subscription order.");
}

{
    var events = new EventBus();
    int damaged = 0;
    int healed = 0;

    events.Subscribe<PlayerDamaged>(_ => damaged++);
    events.Subscribe<PlayerHealed>(_ => healed++);
    events.Publish(new PlayerHealed(1, 5));

    Check(damaged == 0 && healed == 1, "Only listeners of the published type should be called.");
}

{
    var events = new EventBus();

    events.Publish(new PlayerDamaged(1, 5));
    events.Subscribe<PlayerDamaged>(_ => { });
    events.Publish(new PlayerHealed(1, 5));
}

{
    var events = new EventBus();
    int lateCalls = 0;
    void Late(PlayerDamaged _) => lateCalls++;

    events.Subscribe<PlayerDamaged>(_ => events.Subscribe<PlayerDamaged>(Late));
    events.Publish(new PlayerDamaged(1, 5));
    Check(lateCalls == 0, "A listener added during publish should not run in that publish.");

    events.Publish(new PlayerDamaged(1, 5));
    Check(lateCalls == 1, "A listener added during publish should run from the next publish.");
}

{
    var events = new EventBus();
    int secondCalls = 0;
    void Second(PlayerDamaged _) => secondCalls++;

    events.Subscribe<PlayerDamaged>(_ => events.Unsubscribe<PlayerDamaged>(Second));
    events.Subscribe<PlayerDamaged>(Second);

    events.Publish(new PlayerDamaged(1, 5));
    Check(secondCalls == 1, "A listener removed during publish should still finish that publish.");

    events.Publish(new PlayerDamaged(1, 5));
    Check(secondCalls == 1, "A listener removed during publish should not run in the next publish.");
}

{
    var events = new EventBus();
    int calls = 0;

    void Once(PlayerDamaged _)
    {
        calls++;
        events.Unsubscribe<PlayerDamaged>(Once);
    }

    events.Subscribe<PlayerDamaged>(Once);
    events.Publish(new PlayerDamaged(1, 5));
    events.Publish(new PlayerDamaged(1, 5));

    Check(calls == 1, "A listener should be able to unsubscribe itself.");
}

{
    var events = new EventBus();
    int afterCalls = 0;

    events.Subscribe<PlayerDamaged>(_ => throw new InvalidOperationException("first"));
    events.Subscribe<PlayerDamaged>(_ => afterCalls++);
    events.Subscribe<PlayerDamaged>(_ => throw new ArgumentException("second"));

    var exception = ExpectException<AggregateException>(() => events.Publish(new PlayerDamaged(1, 5)));

    Check(afterCalls == 1, "Listeners after a failing listener should still run.");
    Check(exception.InnerExceptions.Count == 2, "Every listener exception should be reported.");
    Check(exception.InnerExceptions[0] is InvalidOperationException
        && exception.InnerExceptions[1] is ArgumentException, "Exceptions should keep the listener order.");
}

{
    var events = new EventBus();

    ExpectException<ArgumentNullException>(() => events.Subscribe<PlayerDamaged>(null!));
    ExpectException<ArgumentNullException>(() => events.Unsubscribe<PlayerDamaged>(null!));
}

Console.WriteLine("Event bus checks passed.");

static void Check(bool condition, string message)
{
    if (!condition)
        throw new Exception(message);
}

static TException ExpectException<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException e)
    {
        return e;
    }

    throw new Exception($"Expected {typeof(TException).Name}.");
}

readonly record struct PlayerDamaged(int PlayerId, int Damage);
readonly record struct PlayerHealed(int PlayerId, int Amount);
