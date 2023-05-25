using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.PublicBoolPropertyNameAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class PublicBoolPropertyNameTests
    {
        [TestMethod]
        public async Task AZM0020PublicBoolTypeWithVerbPrefix()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public bool? EnableFips { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0020PublicBoolTypeWithoutVerbPrefix()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public bool? Fips { get; set; }
    }
}";
            var expected = VerifyCS.Diagnostic(PublicBoolPropertyNameAnalyzer.DiagnosticId).WithSpan(5, 22, 5, 26).WithArguments("Fips");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
