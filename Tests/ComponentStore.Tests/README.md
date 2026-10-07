# Component store tests

Run from the repository root:

```sh
dotnet run --project Tests/ComponentStore.Tests/ComponentStore.Tests.csproj
```

This dependency-free console test runner uses assertions and returns a nonzero
exit code on failure. It compiles the production store, registry, facade, interface, and entity
source files directly so unrelated engine compilation failures do not prevent
testing storage behavior. It does not replace a full solution build.
