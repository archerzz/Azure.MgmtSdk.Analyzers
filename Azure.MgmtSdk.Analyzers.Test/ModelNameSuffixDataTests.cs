using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelNameSuffixDataAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class ModelNameSuffixConditionDataTests
    {
        [TestMethod]
        public async Task ClassUnderNonModelsNamespaceIsNotChecked()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Temp
{
    public partial class AadAuthenticationData
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ModelClassWithDataSuffix()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public partial class AadAuthenticationData
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixDataAnalyzer.DiagnosticId).WithSpan(4, 26, 4, 47).WithArguments("AadAuthenticationData", "Data");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ResourceDataClassesAreNotChecked()
        {
            var test = @"
using Azure.ResourceManager.Models;
namespace Azure.ResourceManager.Models
{
    public class ResourceData {
    }
}
namespace Azure.ResourceManager.Network.Models
{
    public partial class AadAuthenticationData: ResourceData
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TrackedResourceDataClassesAreNotChecked()
        {
            var test = @"
using Azure.ResourceManager.Models;
namespace Azure.ResourceManager.Models
{
    public class TrackedResourceData {
    }
}
namespace Azure.ResourceManager.Network.Models
{
    public partial class AadAuthenticationData: TrackedResourceData
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}

