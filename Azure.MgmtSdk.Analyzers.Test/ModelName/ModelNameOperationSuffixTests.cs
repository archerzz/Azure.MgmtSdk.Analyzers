using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelName.ModelNameOperationSuffixAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;
using Azure.MgmtSdk.Analyzers.ModelName;

namespace Azure.MgmtSdk.Analyzers.Test.ModelName
{
    [TestClass]
    public class ModelNameSuffixOperationTests
    {
        [TestMethod]
        public async Task OperationClassIsNotChecked()
        {
            var test = @"
using Azure;
using Azure.ResourceManager;
namespace Azure
{
    public class Operation
    {
    }
    public class Operation<T> 
    {
    }
}
namespace Azure.ResourceManager
{
    public class ArmOperation : Operation
    {
    }
    public class ArmOperation<T> : Operation<T>
    {
    }
}
namespace Azure.ResourceManager.Network.Models
{
    internal class DnsOperation : Operation 
    {
    }
    internal class DnsArmOperation : ArmOperation 
    {
    }
    internal class DnsOperation<T> : Operation<T> 
    {
    }
    internal class DnsArmOperation<T> : ArmOperation<T> 
    {
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task OperationSuffix()
        {
            var test = @"
namespace Azure.ResourceManager.Network.Models
{
    public class DnsOperation
    {
    }
    public class DnsArmOperation<T>
    {
    }
}";
            DiagnosticResult[] expected = {
                VerifyCS.Diagnostic(ModelNameOperationSuffixAnalyzer.DiagnosticId).WithSpan(4, 18, 4, 30).WithArguments("DnsOperation", "Operation", "DnsData", "DnsInfo"),
                VerifyCS.Diagnostic(ModelNameOperationSuffixAnalyzer.DiagnosticId).WithSpan(7, 18, 7, 33).WithArguments("DnsArmOperation", "Operation", "DnsArmData", "DnsArmInfo")
            };
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}

