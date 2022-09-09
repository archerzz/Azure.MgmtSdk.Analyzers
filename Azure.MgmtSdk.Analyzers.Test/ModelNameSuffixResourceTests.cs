using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffixResourceAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixResourceTests
    {
        [TestMethod]
        public async Task AZM0014WithoutModels()
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
        public async Task AZM0014TestResource()
        {
            var test = @"namespace Test.Models
{
    public class TestResource
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixResourceAnalyzer.DiagnosticId).WithSpan(3, 18, 3, 30).WithArguments("TestResource", "Resource");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0014GenericResource()
        {
            var test = @"namespace Test.Models
{
    public class GenericResource
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0014PrivateLinkServiceResource()
        {
            var test = @"namespace Test.Models
{
    public class PrivateLinkServiceResource
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }
    }
}
