# Azure.MgmtSdk.Analyzers
Analyzers for Azure .NET Management SDK

## Use Tutorials
### Nuget Package
1. Use Visual Studio to open the "Azure.MgmtSdk.Analyzers.sln", build the total solution.
2. Public "Azure.MgmtSdk.Analyzers.Package" in Visual Studio, please remember the output path of "Azure.MgmtSdk.Analyzers.*.nupkg".

### SDK Generation
1. Open ".\NuGet.Config", in "packageSources", add a <key,value> package source, where the key is "Azure.MgmtSdk.Analyzer", while the value is the output path of "Azure.MgmtSdk.Analyzers.*.nupkg" above.
2. Open ".\eng\Packages.Data.props", add a new line of "PackageReference". The default line can be set as <PackageReference Update="Azure.MgmtSdk.Analyzers" Version="0.1.0" PrivateAssets="All"/>. It is required to update the version when the Azure.MgmtSdk.Analyzers is updated.
3. Pick a SDK you want to generate. Here I choose `networkfunction` as an example. Go to the SDK folder, the example folder is ".\sdk\networkfunction\Azure.ResourceManager.NetworkFunction".
4. Open ".\src\Azure.ResourceManager.NetworkFunction.csproj", add three lines before the end of "Project" tag.
```
  <ItemGroup>
    <PackageReference Include="Azure.MgmtSdk.Analyzers"></PackageReference>
  </ItemGroup>
```
5. Command `donet clean` to clean previous settings.
6. Command `dotnet build` to finish the SDK Generation process.