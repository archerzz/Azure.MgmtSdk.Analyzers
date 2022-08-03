using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffixBasicAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixBasicTests
    {
        [TestMethod]
        public async Task AZM0010C0WithoutModels()
        {
            var test = @"using System;

class MonitorResult
{
    static void Main()
    {
        Console.WriteLine(""Hello, world!"");
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0010C0ApplicationGatewayResult()
        {
            var test = @"namespace Test.Models
{
    internal partial class ApplicationGatewayAvailableWafRuleSetsResults
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixBasicAnalyzer.DiagnosticIdBase).WithSpan(3, 28, 3, 73).WithArguments("ApplicationGatewayAvailableWafRuleSetsResults", "Results");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0010C0ResponseParameters()
        {
            var test = @"namespace Test.Models
{
    public class ResponseParameters
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixBasicAnalyzer.DiagnosticIdBase).WithSpan(3, 18, 3, 36).WithArguments("ResponseParameters", "Parameters");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0010C0ClassResult()
        {
            var test = @"namespace ResponseTest.Models
{
    public class ResponseParameter
    {
        static void MainResponseParameter() {
        
        }
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixBasicAnalyzer.DiagnosticIdBase).WithSpan(3, 18, 3, 35).WithArguments("ResponseParameter", "Parameter");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0010C0TwoNamespace()
        {
            var test = @"namespace NamespaceTest.Models
{
    namespace SubTest
    {
        public class ResponseParameter
        {
            static void MainResponseParameter() {
        
            }
        }
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixBasicAnalyzer.DiagnosticIdBase).WithSpan(5, 22, 5, 39).WithArguments("ResponseParameter", "Parameter");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
