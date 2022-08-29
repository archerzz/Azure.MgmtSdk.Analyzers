using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffixDefinitionAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixDefinitionTests
    {
        [TestMethod]
        public async Task AZM0011PartialClassDefinition()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public partial class AadAuthenticationDefinition
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixDefinitionAnalyzer.DiagnosticId).WithSpan(4, 26, 4, 53).WithArguments("AadAuthenticationDefinition", "Definition");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0011DefinitionInherit()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public class ArmResource {
    }
    public partial class AadAuthenticationDefinition: ArmResource
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }
   
    }
}

