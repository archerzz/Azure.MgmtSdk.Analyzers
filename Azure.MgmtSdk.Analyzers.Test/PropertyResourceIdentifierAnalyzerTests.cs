using Azure.MgmtSdk.Analyzers.PropertyType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.PropertyType.PropertyResourceIdentifierAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class PropertyResourceIdentifierAnalyzerTests
    {
        [TestMethod]
        public async Task ValidCases()
        {
            var test1 = @"
namespace Azure.ResourceManager.Network
{
    public class ResourceIdentifier
    {
    }
    public partial class Test
    {
        public ResourceIdentifier DefaultOriginGroupId { get; set; }
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
        public string DefaultOriginGroupResourceIdentifier { get; set; }
    }
}";
            var test2 = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public static readonly string DefaultOriginGroupResourceIdentifier; 
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test1, VerifyCS.Diagnostic(PropertyResourceIdentifierAnalyzer.DiagnosticId).WithSpan(5, 23, 5, 59).WithArguments("DefaultOriginGroupResourceIdentifier", "string"));
            await VerifyCS.VerifyAnalyzerAsync(test2, VerifyCS.Diagnostic(PropertyResourceIdentifierAnalyzer.DiagnosticId).WithSpan(5, 39, 5, 75).WithArguments("DefaultOriginGroupResourceIdentifier", "string"));
        }
    }
}
