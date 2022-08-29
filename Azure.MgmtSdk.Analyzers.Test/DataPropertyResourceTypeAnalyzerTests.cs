using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.DataPropertyResourceTypeAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class DataPropertyResourceTypeAnalyzerTests
    {
        [TestMethod]
        public async Task AZM0042DataPropertyResourceTypeCorrectType()
        {
            var test = @"
namespace Azure.ResourceManager.Network
{
    public class ResourceType
    {
    }
    public partial class Test
    {
        public static readonly ResourceType ResourceType;
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0042DataPropertyResourceTypeIncorrectType()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public string ResourceType { get; set; }
    }
}";
            var expected = VerifyCS.Diagnostic(DataPropertyResourceTypeAnalyzer.DiagnosticId).WithSpan(5, 23, 5, 35).WithArguments("ResourceType", "string");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0042DataPropertyResourceTypeIncorrectVariableType()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public static readonly string ResourceType;    
    }
}";
            var expected = VerifyCS.Diagnostic(DataPropertyResourceTypeAnalyzer.DiagnosticId).WithSpan(5, 39, 5, 51).WithArguments("ResourceType", "string");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
