using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

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
    }
    public partial class AadAuthenticationDefinition: ArmResource
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0011DataNoModels()
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
        public async Task AZM0011DataWithModels()
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
            var expected = VerifyCS.Diagnostic(ModelNameSuffixCondition.DiagnosticId).WithSpan(6, 26, 6, 47).WithArguments("AadAuthenticationData", "Data");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0011OperationTemplate()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public class ArmOperation<T> 
    {
    }
    internal class DnsArmOperation<T> : ArmOperation<T> 
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0011Operation()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public class ArmOperation
    {
    }
    internal class DnsArmOperation : ArmOperation
    {
    }
}";
            DiagnosticResult[] expected = { VerifyCS.Diagnostic(ModelNameSuffixCondition.DiagnosticId).WithSpan(4, 18, 4, 30).WithArguments("ArmOperation", "Operation"), VerifyCS.Diagnostic(ModelNameSuffixCondition.DiagnosticId).WithSpan(7, 20, 7, 35).WithArguments("DnsArmOperation", "Operation") };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}

