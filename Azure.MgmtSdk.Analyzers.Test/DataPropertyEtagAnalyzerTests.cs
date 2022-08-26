using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.DataPropertyEtagAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class DataPropertyEtagTests
    {
        [TestMethod]
        public async Task AZM0043DataPropertyEtagCorrectVariableType()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public class ETag
        {
        }
#nullable enable
        public ETag? Etag { get; }
#nullable disable
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0043DataPropertyEtagInCorrectPropertyType()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public string Etag { get; set; }
    }
}";
            var expected = VerifyCS.Diagnostic(DataPropertyEtagAnalyzer.DiagnosticIdDataPropertyETagName).WithSpan(5, 23, 5, 27).WithArguments("Etag", "string");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task AZM0043DataPropertyEtagIncorrectVariableType()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public static readonly string Etag;    
    }
}";
            var expected = VerifyCS.Diagnostic(DataPropertyEtagAnalyzer.DiagnosticIdDataPropertyETagName).WithSpan(5, 39, 5, 43).WithArguments("Etag", "string");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
