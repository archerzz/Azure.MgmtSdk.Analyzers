using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.DataPropertyAzureLocationAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class DataPropertyAzureLocationAnalyzerTests
    {
        [TestMethod]
        public async Task ValidCases()
        {
            var test1 = @"
namespace Azure.ResourceManager.Network
{
    public class AzureLocation
    {
    }
    public partial class Test
    {
#nullable enable
        public AzureLocation? Location { get; }
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
        public string Location { get; set; }
    }
}";
            //            var test2 = @"namespace Azure.ResourceManager.Network
            //{
            //    public partial class Test
            //    {
            //        public static readonly string Location; 
            //    }
            //}";
            await VerifyCS.VerifyAnalyzerAsync(test1, VerifyCS.Diagnostic(DataPropertyAzureLocationAnalyzer.DiagnosticId).WithSpan(5, 23, 5, 31).WithArguments("Location", "string"));
            //await VerifyCS.VerifyAnalyzerAsync(test2, VerifyCS.Diagnostic(DataPropertyAzureLocationAnalyzer.DiagnosticId).WithSpan(5, 39, 5, 47).WithArguments("Location", "string"));
        }
    }
}
