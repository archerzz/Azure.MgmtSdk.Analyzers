using Azure.MgmtSdk.Analyzers.PropertyType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.PropertyType.PropertyResourceTypeAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class PropertyResourceTypeAnalyzerTests
    {
        [TestMethod]
        public async Task ValidCases()
        {
            var test1 = @"
namespace Azure.ResourceManager.Network
{
    public class ResourceType
    {
    }
    public partial class Test
    {
        public static readonly ResourceType ResourceType;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test1);
        }

        [TestMethod]
        public async Task ErrorCases()
        {
            var test1 = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public string ResourceType { get; set; }
    }
}";
            var test2 = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public static readonly string ResourceType;    
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test1, VerifyCS.Diagnostic(PropertyResourceTypeAnalyzer.DiagnosticId).WithSpan(5, 23, 5, 35).WithArguments("ResourceType", "string"));
            await VerifyCS.VerifyAnalyzerAsync(test2, VerifyCS.Diagnostic(PropertyResourceTypeAnalyzer.DiagnosticId).WithSpan(5, 39, 5, 51).WithArguments("ResourceType", "string"));
        }
    }
}
