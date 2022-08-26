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
        public async Task ValidCases()
        {
            var test = @"using System.Collections.Generic;
using System.Collections;

namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public int TestIntervalInSeconds { get; set; }
        public int TestDurationInSeconds { get; set; }
        public int Foo { get; set; }
        public bool TestInterval { get; set; }
        public string StringInterval { get; set; }
        public double DoubleInterval { get; set; }
        public IList<string> IListInterval { get; set; }
        public int IntervalInSeconds { get; set; }
        public int IntervalInMinutes { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test); // Default No errors.
        }

        [TestMethod]
        public async Task ErrorCases()
        {
            var test = @"namespace Azure.ResourceManager.Network
{
    public partial class Test
    {
        public int FipInterval { get; set; }
        public int FipDuration { get; set; }
        public uint Interval { get; set; }
        public short Duration { get; set; }
        public ushort Uint16Interval { get; set; }
        public long Int64Interval { get; set; }
        public ulong Uint64Duration { get; set; }
    }
}";
            await VerifyCS.VerifyAnalyzerAsync(test,
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(5, 20, 5, 31).WithArguments("FipInterval"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(6, 20, 6, 31).WithArguments("FipDuration"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(7, 21, 7, 29).WithArguments("Interval"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(8, 22, 8, 30).WithArguments("Duration"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(9, 23, 9, 37).WithArguments("Uint16Interval"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(10, 21, 10, 34).WithArguments("Int64Interval"),
                VerifyCS.Diagnostic(IntervalPropertyNameAnalyzer.DiagnosticId).WithSpan(11, 22, 11, 36).WithArguments("Uint64Duration")
            );
        }
    }
}
