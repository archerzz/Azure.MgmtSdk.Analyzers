using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.TypeNameSuffixAnalyzer,
    Azure.MgmtSdk.Analyzers.TypeNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class TypeNameSuffixAnalyzerTests
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
            var expected = VerifyCS.Diagnostic(TypeNameSuffixAnalyzer.DiagnosticId).WithSpan(3, 7, 3, 20).WithArguments("MonitorResult", "Result");
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
            var expected = VerifyCS.Diagnostic(TypeNameSuffixAnalyzer.DiagnosticId).WithSpan(3, 28, 3, 73).WithArguments("ApplicationGatewayAvailableWafRuleSetsResults", "Results");
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
            var expected = VerifyCS.Diagnostic(TypeNameSuffixAnalyzer.DiagnosticId).WithSpan(3, 18, 3, 36).WithArguments("ResponseParameters", "Parameters");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
