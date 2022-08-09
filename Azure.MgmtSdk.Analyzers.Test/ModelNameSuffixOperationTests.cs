using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffixOperationAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixOperationTests
    {
        [TestMethod]
        public async Task AZM0013OperationTemplate()
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
        public async Task AZM0013Operation()
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
            DiagnosticResult[] expected = { VerifyCS.Diagnostic(ModelNameSuffixOperationAnalyzer.DiagnosticIdOperation).WithSpan(4, 18, 4, 30).WithArguments("ArmOperation", "Operation"), VerifyCS.Diagnostic(ModelNameSuffixOperationAnalyzer.DiagnosticIdOperation).WithSpan(7, 20, 7, 35).WithArguments("DnsArmOperation", "Operation") };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}

