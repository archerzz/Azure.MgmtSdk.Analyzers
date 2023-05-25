using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.PropertyType.PropertyEtagAnalyzer>;
using Azure.MgmtSdk.Analyzers.PropertyType;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class PropertyEtagTests
    {
        [TestMethod]
        public async Task ValidCases()
        {
            var test1 = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public class ETag
        {
        }
#nullable enable
        public ETag? Etag { get; }
#nullable disable
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
        public string Etag { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test1, VerifyCS.Diagnostic(PropertyEtagAnalyzer.DiagnosticId).WithSpan(5, 23, 5, 27).WithArguments("Etag", "string"));
        }

    }
}
