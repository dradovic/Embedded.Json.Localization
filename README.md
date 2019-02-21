# Embedded.Json.Localization

## Description

This project allows you to use JSON files for .NET Core Localization instead of resx files.
Special feature: works with *multiple* projects/assemblies (i.e. the resources can stay in
the project where they belong).

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

You can find a fully working sample in the source code.


NuGet Package: [![NuGet](https://img.shields.io/nuget/v/Embedded.Json.Localization.svg)](https://www.nuget.org/packages/Embedded.Json.Localization/1.0.0)