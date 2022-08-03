# Azure.MgmtSdk.Analyzers
Analyzers for Azure .NET Management SDK

## Use Tutorials

<<<<<<< HEAD
### Using Project Reference
Take NetworkFunction as an example.
1. Open ".\src\Azure.ResourceManager.NetworkFunction.csproj", add three lines before the end of "Project" tag. Remember to replace "{your_path}" by your local project path.
```
  <ItemGroup>
    <ProjectReference Include="{your_path}\Azure.MgmtSdk.Analyzers\Azure.MgmtSdk.Analyzers\Azure.MgmtSdk.Analyzers.csproj" PrivateAssets="all" ReferenceOutputAssembly="false" OutputItemType="Analyzer"/>
  </ItemGroup>
```
2. Command `dotnet build` in command line or use `Build` in Visual Studio to finish the SDK Generation process.


=======
>>>>>>> e7cdca13b88fe3c294f0c899c3c3fa9ad93d8175
### Using Nuget Package
#### Nuget Package
1. Use Visual Studio to open the "Azure.MgmtSdk.Analyzers.sln", build the total solution.
2. Public "Azure.MgmtSdk.Analyzers.Package" in Visual Studio, please remember the output path of "Azure.MgmtSdk.Analyzers.*.nupkg".

#### SDK Generation
1. Open ".\NuGet.Config", in "packageSources", add a <key,value> package source, where the key is "Azure.MgmtSdk.Analyzer", while the value is the output path of "Azure.MgmtSdk.Analyzers.*.nupkg" above.
2. Open ".\eng\Packages.Data.props", add a new line of "PackageReference". The default line can be set as <PackageReference Update="Azure.MgmtSdk.Analyzers" Version="0.1.0" PrivateAssets="All"/>. It is required to update the version when the Azure.MgmtSdk.Analyzers is updated.
3. Pick a SDK you want to generate. Here I choose `networkfunction` as an example. Go to the SDK folder, the example folder is ".\sdk\networkfunction\Azure.ResourceManager.NetworkFunction".
4. Open ".\src\Azure.ResourceManager.NetworkFunction.csproj", add three lines before the end of "Project" tag.
```
  <ItemGroup>
    <PackageReference Include="Azure.MgmtSdk.Analyzers"></PackageReference>
  </ItemGroup>
```
5. Command `dotnet clean; dotnet build` in command line to clean previous settings and finish the SDK Generation process.
<<<<<<< HEAD
=======

### Using Project Reference
1. Open ".\src\Azure.ResourceManager.NetworkFunction.csproj", add three lines before the end of "Project" tag. Remember to replace "{your_path}" by your local project path.
```
  <ProjectReference Include="{your_path}\Azure.MgmtSdk.Analyzers\Azure.MgmtSdk.Analyzers\Azure.MgmtSdk.Analyzers.csproj" PrivateAssets="all" ReferenceOutputAssembly="false" OutputItemType="Analyzer"/>
  </ItemGroup>
```
2. Command `dotnet build` in command line or use `Build` in Visual Studio to finish the SDK Generation process.
>>>>>>> e7cdca13b88fe3c294f0c899c3c3fa9ad93d8175
