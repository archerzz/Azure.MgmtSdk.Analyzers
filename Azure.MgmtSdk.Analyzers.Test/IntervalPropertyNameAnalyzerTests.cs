using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpAnalyzerVerifier<
    Azure.MgmtSdk.Analyzers.IntervalPropertyNameAnalyzer>;

namespace Azure.MgmtSdk.Analyzers.Test
{
    [TestClass]
    public class IntervalPropertyNameAnalyzerTests
    {
        [TestMethod]
        public async Task AZM0020PublicBoolTypeWithVerbPrefix()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public int TestIntervalInSeconds { get; set; }
        public int TestDurationInSeconds { get; set; }
        public int Foo { get; set; }
        public bool TestInterval { get; set; }
        public int IntervalInSeconds { get; set; }
        public int IntervalInMinutes { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task AZM0020PublicBoolTypeWithoutVerbPrefix()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public int FipInterval { get; set; }
        public int FipDuration { get; set; }
        public int Interval { get; set; }
        public int Duration { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test,
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(5, 20, 5, 31).WithArguments("FipInterval"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(6, 20, 6, 31).WithArguments("FipDuration"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(7, 20, 7, 28).WithArguments("Interval"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(8, 20, 8, 28).WithArguments("Duration")
            );
        }
    }
}
