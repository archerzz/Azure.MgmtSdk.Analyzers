using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCondition,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixConditionTests
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
            var expected = VerifyCS.Diagnostic(ModelNameSuffixCondition.DiagnosticId).WithSpan(4, 26, 4, 53).WithArguments("AadAuthenticationDefinition", "Definition");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0011DefinitionInherit()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public class ArmResource {
        protected int width;
        protected int height;
    }
    public partial class AadAuthenticationDefinition: ArmResource
    {
      public int getArea()
      {
         return (width * height);
      }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }
    }
}
