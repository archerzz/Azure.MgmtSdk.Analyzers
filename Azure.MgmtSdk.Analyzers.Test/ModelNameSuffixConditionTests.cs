using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffix,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixConditionTests
    {
        [TestMethod]
        public async Task AZM0010NameEndWithResult()
        {
            var test = @"namespace Azure.ResourceManager.Network.Models
{
    public partial class AadAuthenticationParameters : IUtf8JsonSerializable
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffix.DiagnosticId).WithSpan(3, 26, 3, 53).WithArguments("MonitorParameters", "Parameters");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

    }
}
