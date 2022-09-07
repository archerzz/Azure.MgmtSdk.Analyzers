using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffixDataAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixConditionDataTests
    {
        [TestMethod]
        public async Task AZM0012DataNoModels()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Temp
{
    public class ArmResource {
    }
    public partial class AadAuthenticationData: ArmResource
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0012DataWithModels()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public class ArmResource {
    }
    public partial class AadAuthenticationData: ArmResource
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixDataAnalyzer.DiagnosticId).WithSpan(6, 26, 6, 47).WithArguments("AadAuthenticationData", "Data");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0012DataBaseTrackedResourceData()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public class TrackedResourceData {
    }
    public partial class AadAuthenticationData: TrackedResourceData
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

    }
}

