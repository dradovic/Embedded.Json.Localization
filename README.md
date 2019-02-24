# Embedded.Json.Localization

## Description

This project allows you to use JSON files for .NET Core Localization instead of resx files.
Special features: 
1. Works with *multiple* projects/assemblies (i.e. the resources can stay in
the project where they belong).
1. Automatic fallback to parent culture (e.g. if a resource is not found in 'de-CH',
it is also searched for in 'de' resources).

## Configuration

1. Add the package reference:
```bash
    dotnet add package Embedded.Json.Localization --version x.x.x
```

2. In Startup.cs:
```cs
    services.AddJsonLocalization();
```

3. Embed the json in the corresponding project files (csproj). For example:
```csproj
  <ItemGroup>
    <EmbeddedResource Include="**/*.de-CH.json" />
  </ItemGroup>
```

## Example

You can find a fully working sample in the source code. Note that the cookie consent has been localized (to
show an instance of view localization) and the header messages of the About and Contact screens (to have an example of localization happening in a controller inlcuding fallback to parent culture).


NuGet Package: [![NuGet](https://img.shields.io/nuget/v/Embedded.Json.Localization.svg)](https://www.nuget.org/packages/Embedded.Json.Localization/1.0.0)