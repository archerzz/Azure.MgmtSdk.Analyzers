using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffix,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixTests
    {
        [TestMethod]
        public async Task AZM0010NameEndWithResult()
        {
            var test = @"using System;

class MonitorResult
{
    static void Main()
    {
        Console.WriteLine(""Hello, world!"");
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffix.DiagnosticId).WithSpan(3, 7, 3, 20).WithArguments("MonitorResult", "Result");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0010ApplicationGatewayResult()
        {
            var test = @"namespace Test
{
    internal partial class ApplicationGatewayAvailableWafRuleSetsResults
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffix.DiagnosticId).WithSpan(3, 28, 3, 73).WithArguments("ApplicationGatewayAvailableWafRuleSetsResults", "Results");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0010ResponseParameters()
        {
            var test = @"namespace Test
{
    public class ResponseParameters
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffix.DiagnosticId).WithSpan(3, 18, 3, 36).WithArguments("ResponseParameters", "Parameters");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0010ClassResult()
        {
            var test = @"namespace ResponseParameters
{
    public class ResponseParameter
    {
        static void MainResponseParameter() {
        
        }
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffix.DiagnosticId).WithSpan(3, 18, 3, 35).WithArguments("ResponseParameter", "Parameter");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
