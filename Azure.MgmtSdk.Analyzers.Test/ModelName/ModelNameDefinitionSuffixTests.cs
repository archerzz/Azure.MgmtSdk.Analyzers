using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelName.ModelNameDefinitionSuffixAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;
using Azure.MgmtSdk.Analyzers.ModelName;

namespace Azure.MgmtSdk.Analyzers.Test.ModelName
{
    [TestClass]
    public class ModelNameSuffixDefinitionTests
    {
        [TestMethod]
        public async Task ModelWithDefinitionSuffix()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public partial class AadAuthenticationDefinition
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameDefinitionSuffixAnalyzer.DiagnosticId).WithSpan(4, 26, 4, 53).WithArguments("AadAuthenticationDefinition", "Definition");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ArmResourceIsNotChecked()
        {
            var test = @"
using Azure.ResourceManager;
namespace Azure.ResourceManager
{
    public class ArmResource {
    }
}
namespace Azure.ResourceManager.Network.Models
{
    public partial class AadAuthenticationDefinition: ArmResource
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

    }
}

