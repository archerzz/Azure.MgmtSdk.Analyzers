using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.DataPropertyResourceIdentifierAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class DataPropertyResourceIdentifierAnalyzerTests
    {
        [TestMethod]
        public async Task AZM0041DataPropertyResourceIdentifierCorrectPropertyType()
        {
            var test = @"
namespace Azure.ResourceManager.Network
{
    public class ResourceIdentifier
    {
    }
    public partial class Test
    {
        public ResourceIdentifier DefaultOriginGroupId { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0041DataPropertyResourceIdentifierIncorrectPropertyType()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public string DefaultOriginGroupId { get; set; }
    }
}";
            var expected = VerifyCS.Diagnostic(DataPropertyResourceIdentifierAnalyzer.DiagnosticIdDataPropertyResourceIdentifierName).WithSpan(5, 23, 5, 43).WithArguments("DefaultOriginGroupId", "string");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0041DataPropertyResourceIdentifierIncorrectVariableType()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public static readonly string DefaultOriginGroupId; 
    }
}";
            var expected = VerifyCS.Diagnostic(DataPropertyResourceIdentifierAnalyzer.DiagnosticIdDataPropertyResourceIdentifierName).WithSpan(5, 39, 5, 59).WithArguments("DefaultOriginGroupId", "string");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
