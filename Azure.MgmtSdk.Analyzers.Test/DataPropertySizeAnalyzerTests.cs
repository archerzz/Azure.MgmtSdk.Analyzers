using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.DataPropertySizeAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class DataPropertySizeAnalyzerTests
    {
        [TestMethod]
        public async Task ValidCases()
        {
            var test1 = @"
namespace Azure.ResourceManager.Network
{
    public class VmSize
    {
    }
    public partial class Test
    {
        int? pageSizeHint;
        public int? osDiskSizeGB { get; set; }
#nullable enable
        public VmSize? VmSize { get; }
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
        public string pageSizeHint { get; set; }
    }
}";
            var test2 = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public static readonly string pageSizeHint; 
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test1, VerifyCS.Diagnostic(DataPropertySizeAnalyzer.DiagnosticId).WithSpan(5, 23, 5, 35).WithArguments("pageSizeHint", "string"));
            await VerifyCS.VerifyAnalyzerAsync(test2, VerifyCS.Diagnostic(DataPropertySizeAnalyzer.DiagnosticId).WithSpan(5, 39, 5, 51).WithArguments("pageSizeHint", "string"));
        }
    }
}
