using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.DataPropertyEtagAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class DataPropertyEtagTests
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
            await VerifyCS.VerifyAnalyzerAsync(test1, VerifyCS.Diagnostic(DataPropertyEtagAnalyzer.DiagnosticId).WithSpan(5, 23, 5, 27).WithArguments("Etag", "string"));
            //await VerifyCS.VerifyAnalyzerAsync(test2, VerifyCS.Diagnostic(DataPropertyEtagAnalyzer.DiagnosticId).WithSpan(5, 39, 5, 43).WithArguments("Etag", "string"));
        }

    }
}
