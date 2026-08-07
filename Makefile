.PHONY: run build restore clean format

run:
	dotnet run --project Bootstrap/Bootstrap.csproj

build:
	dotnet build

restore:
	dotnet restore

clean:
	dotnet clean

format:
	dotnet format