# Azure.MgmtSdk.Analyzers
Code Analyzers for Azure .NET Management SDK, using Roslyn.

## Use Tutorials

### Using Project Reference
Take NetworkFunction as an example.
1. Open ".\src\Azure.ResourceManager.NetworkFunction.csproj", add three lines before the end of "Project" tag. Remember to replace "{your_path}" by your local project path.
```
  <ItemGroup>
    <ProjectReference Include="{your_path}\Azure.MgmtSdk.Analyzers\Azure.MgmtSdk.Analyzers\Azure.MgmtSdk.Analyzers.csproj" PrivateAssets="all" ReferenceOutputAssembly="false" OutputItemType="Analyzer"/>
  </ItemGroup>
```
2. Command `dotnet build` in command line or use `Build` in Visual Studio to finish the SDK Generation process.


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


## Rules

### Finished
- [x] Avoid using Parameter(s), Request, Options and Response as the suffix in model.
- [x] Avoid using reserved names Collection, Resource as model suffix. Avoid using Data as model suffix unless the model derives from ResourceData/TrackedResourceData. 
- [x] Avoid using Definition as model suffix unless it's the name of a Resource. 
- [x] Avoid using Operation as model suffix unless the model derives from Operation<T>.
- [x] Property with type bool should start with a verb including 'Is', 'Can', 'Has', 'Enable' prefix.
- [x] The data type of a property should be 'ResourceIdentifier' if its name is end of 'Id' and value is really a ResourceIdentifier.
- [x] The data type of a property should be 'ResouceType' if its name is 'ResourceType' or end with 'Type' and the value is really a ResourceType.
- [x] The data type of a property should be 'ETag' if its name is 'etag' and the value is really a ETag. The property name  should be 'ETag'.
- [x] The data type of a property might be 'int'/'long' if its name contains 'size'. Try to update the property name if it's a string.
- [x] The data type of a property might be 'AzureLocation' if its name is end with 'location' / 'locations'.