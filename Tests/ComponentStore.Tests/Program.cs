using MiniEngine.Components.Core;
using MiniEngine.Entities;

namespace MiniEngine.ComponentStoreTests;

internal static class Program
{
    private static int Main()
    {
        (string Name, Action Run)[] tests =
        [
            ("Add, Get, and Has", AddAndGet),
            ("Ref mutation persists", RefMutation),
            ("Shared interface supports membership and removal", SharedInterface),
            ("Generic interface supports typed ref access and count", GenericInterface),
            ("Manager preserves typed stores and ref access", ManagerRefMutation),
            ("References survive additions within capacity", RefWithinCapacity),
            ("Missing and invalid IDs", MissingAndInvalidIds),
            ("Duplicate Add preserves original data", DuplicateAdd),
            ("Remove and swap-and-pop", SwapAndPop),
            ("Remove last and only component", RemoveLast),
            ("Add after removals", Reinsert),
            ("Sparse growth initializes missing slots", SparseGrowth),
            ("Dense growth preserves data and ref mutation", DenseGrowth),
            ("Mixed operations match a dictionary", MixedOperations)
        ];

        var failures = 0;
        foreach (var test in tests)
        {
            try
            {
                test.Run();
                Console.WriteLine($"PASS: {test.Name}");
            }
            catch (Exception exception)
            {
                failures++;
                Console.Error.WriteLine($"FAIL: {test.Name}: {exception}");
            }
        }

        Console.WriteLine($"{tests.Length - failures}/{tests.Length} tests passed.");
        return failures == 0 ? 0 : 1;
    }

    private static Entity EntityAt(int id) => new(id, 1);

    private static void AddAndGet()
    {
        var store = new ComponentStore<TestComponent>();
        Check(!store.Has(EntityAt(0)));
        store.Add(EntityAt(2), new TestComponent { Value = 42, Label = "player" });
        Check(store.Has(EntityAt(2)));
        Check(!store.Has(EntityAt(0)));
        Check(store.Get(EntityAt(2)).Value == 42);
        Check(store.Get(EntityAt(2)).Label == "player");
    }

    private static void RefMutation()
    {
        var store = new ComponentStore<TestComponent>();
        var entity = EntityAt(0);
        store.Add(entity, new TestComponent { Value = 1 });
        ref var component = ref store.Get(entity);
        component.Value = 99;
        component.Label = "changed";
        ref var sameComponent = ref store.Get(entity);
        Check(sameComponent.Value == 99 && sameComponent.Label == "changed");
    }

    private static void SharedInterface()
    {
        var typedStore = new ComponentStore<TestComponent>();
        typedStore.Add(EntityAt(0), default);
        IComponentStore store = typedStore;
        Check(store.Has(EntityAt(0)));
        store.Remove(EntityAt(0));
        Check(!typedStore.Has(EntityAt(0)));
    }

    private static void ManagerRefMutation()
    {
        var manager = new ComponentStoreManager();
        var store = manager.Get<TestComponent>();
        store.Add(EntityAt(0), default);
        ref var component = ref manager.Get<TestComponent>().Get(EntityAt(0));
        component.Value = 17;
        Check(ReferenceEquals(store, manager.Get<TestComponent>()));
        Check(manager.Get<TestComponent>().Get(EntityAt(0)).Value == 17);
        var otherStore = manager.Get<OtherComponent>();
        otherStore.Add(EntityAt(0), new OtherComponent { Value = 23 });
        Check(otherStore.Get(EntityAt(0)).Value == 23);
        Check(store.Get(EntityAt(0)).Value == 17);
    }

    private static void GenericInterface()
    {
        IComponentStore<TestComponent> store = new ComponentStore<TestComponent>();
        IComponentStore shared = store;
        Check(shared.Count == 0);
        store.Add(EntityAt(0), new TestComponent { Value = 7 });
        store.Add(EntityAt(1), new TestComponent { Value = 9 });
        Check(shared.Count == 2 && shared.Has(EntityAt(0)));
        ref var component = ref store.Get(EntityAt(1));
        component.Value = 23;
        Check(store.Get(EntityAt(1)).Value == 23);
        shared.Remove(EntityAt(0));
        Check(shared.Count == 1 && !store.Has(EntityAt(0)));
        Check(store.Get(EntityAt(1)).Value == 23);
        shared.Remove(EntityAt(0));
        Check(shared.Count == 1);
        store.Remove(EntityAt(1));
        Check(shared.Count == 0);
    }

    private static void RefWithinCapacity()
    {
        var store = new ComponentStore<TestComponent>();
        store.Add(EntityAt(0), default);
        ref var component = ref store.Get(EntityAt(0));
        store.Add(EntityAt(1000), default);
        component.Value = 31;
        Check(store.Get(EntityAt(0)).Value == 31);
    }

    private static void MissingAndInvalidIds()
    {
        var store = new ComponentStore<TestComponent>();
        foreach (var id in new[] { -1, 0, int.MaxValue })
        {
            Check(!store.Has(EntityAt(id)));
            Throws<InvalidOperationException>(() => { store.Get(EntityAt(id)); });
            store.Remove(EntityAt(id));
        }
        Throws<ArgumentOutOfRangeException>(() => store.Add(EntityAt(-1), default));
        Throws<ArgumentOutOfRangeException>(() => store.Add(EntityAt(int.MaxValue), default));
        store.Add(EntityAt(0), default);
        store.Remove(EntityAt(0));
        Throws<InvalidOperationException>(() => { store.Get(EntityAt(0)); });
    }

    private static void DuplicateAdd()
    {
        var store = new ComponentStore<TestComponent>();
        store.Add(EntityAt(1), new TestComponent { Value = 3 });
        Throws<InvalidOperationException>(() => store.Add(EntityAt(1), new TestComponent { Value = 9 }));
        Check(store.Get(EntityAt(1)).Value == 3);
        store.Remove(EntityAt(1));
        Check(!store.Has(EntityAt(1)));
        store.Add(EntityAt(1), new TestComponent { Value = 12 });
        Check(store.Get(EntityAt(1)).Value == 12);
    }

    private static void SwapAndPop()
    {
        var store = new ComponentStore<TestComponent>();
        for (var id = 0; id < 4; id++)
            store.Add(EntityAt(id), new TestComponent { Value = id + 10 });
        store.Remove(EntityAt(1));
        Check(!store.Has(EntityAt(1)));
        Check(store.Get(EntityAt(3)).Value == 13);
        Check(store.Get(EntityAt(0)).Value == 10 && store.Get(EntityAt(2)).Value == 12);
        ref var moved = ref store.Get(EntityAt(3));
        moved.Value = 30;
        Check(store.Get(EntityAt(3)).Value == 30);
        store.Remove(EntityAt(3));
        Check(store.Get(EntityAt(2)).Value == 12);
        store.Remove(EntityAt(0));
        Check(store.Get(EntityAt(2)).Value == 12);
    }

    private static void RemoveLast()
    {
        var store = new ComponentStore<TestComponent>();
        store.Add(EntityAt(0), new TestComponent { Value = 10 });
        store.Add(EntityAt(1), default);
        store.Remove(EntityAt(1));
        Check(!store.Has(EntityAt(1)) && store.Get(EntityAt(0)).Value == 10);
        store.Remove(EntityAt(0));
        store.Remove(EntityAt(0));
        Check(!store.Has(EntityAt(0)));
    }

    private static void Reinsert()
    {
        var store = new ComponentStore<TestComponent>();
        for (var id = 0; id < 12; id++)
            store.Add(EntityAt(id), new TestComponent { Value = id });
        for (var id = 0; id < 12; id++)
            store.Remove(EntityAt(id));
        for (var id = 11; id >= 0; id--)
            store.Add(EntityAt(id), new TestComponent { Value = id * 2 });
        for (var id = 0; id < 12; id++)
            Check(store.Get(EntityAt(id)).Value == id * 2);
    }

    private static void SparseGrowth()
    {
        var store = new ComponentStore<TestComponent>();
        store.Add(EntityAt(0), new TestComponent { Value = 7 });
        store.Add(EntityAt(100_000), new TestComponent { Value = 8 });
        Check(store.Get(EntityAt(0)).Value == 7 && store.Get(EntityAt(100_000)).Value == 8);
        for (var id = 1; id < 100_000; id++)
            Check(!store.Has(EntityAt(id)));
        store.Add(EntityAt(200_000), new TestComponent { Value = 9 });
        Check(!store.Has(EntityAt(100_001)));
        Check(store.Get(EntityAt(200_000)).Value == 9);
    }

    private static void DenseGrowth()
    {
        var store = new ComponentStore<TestComponent>();
        for (var id = 0; id < 257; id++)
            store.Add(EntityAt(id), new TestComponent { Value = id });
        for (var id = 0; id < 257; id++)
        {
            ref var component = ref store.Get(EntityAt(id));
            Check(component.Value == id);
            component.Value += 1000;
        }
        for (var id = 0; id < 257; id++)
            Check(store.Get(EntityAt(id)).Value == id + 1000);
    }

    private static void MixedOperations()
    {
        var store = new ComponentStore<TestComponent>();
        var expected = new Dictionary<int, int>();
        var random = new Random(12345);
        for (var step = 0; step < 5000; step++)
        {
            var id = random.Next(128);
            switch (random.Next(3))
            {
                case 0 when !expected.ContainsKey(id):
                    store.Add(EntityAt(id), new TestComponent { Value = step });
                    expected.Add(id, step);
                    break;
                case 1:
                    store.Remove(EntityAt(id));
                    expected.Remove(id);
                    break;
                case 2 when expected.ContainsKey(id):
                    ref var component = ref store.Get(EntityAt(id));
                    component.Value++;
                    expected[id]++;
                    break;
            }
            for (var candidate = 0; candidate < 128; candidate++)
            {
                Check(store.Has(EntityAt(candidate)) == expected.ContainsKey(candidate));
                if (expected.TryGetValue(candidate, out var value))
                    Check(store.Get(EntityAt(candidate)).Value == value);
            }
        }
    }

    private static void Check(bool condition, [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(condition))] string? expression = null)
    {
        if (!condition)
            throw new InvalidOperationException($"Assertion failed: {expression}");
    }

    private static void Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }
        throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
    }

    private struct TestComponent
    {
        public int Value;
        public string? Label;
    }

    private struct OtherComponent
    {
        public int Value;
    }
}
