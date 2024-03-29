﻿using Azure.MgmtSdk.Analyzers.ModelName;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = AzureMgmtSDKAnalyzer.Test.CSharpCodeFixVerifier<
    Azure.MgmtSdk.Analyzers.ModelName.ModelNameSuffixGeneralAnalyzer,
    Azure.MgmtSdk.Analyzers.ModelNameSuffixCodeFixProvider>;

namespace Azure.MgmtSdk.Analyzers.Test.ModelName
{
    [TestClass]
    public class ModelNameSuffixGeneralTests
    {
        [TestMethod]
        public async Task ClassNotUnderModelsNamespaceIsNotChecked()
        {
            var test = @"namespace Azure.ResourceManager;

class MonitorResult
{
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task OnlyModelsNamespaceIsChecked()
        {
            var test = @"namespace Azure.ResourceManager.AModels;

class MonitorResult
{
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task EnumIsNotChecked()
        {
            var test = @"namespace Azure.ResourceManager.Models;

enum MonitorResult
{
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task GoodSuffix()
        {
            var test = @"namespace Azure.ResourceManager.Models;

class MonitorContent
{
}";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ParametersSuffix()
        {
            var test = @"namespace Azure.ResourceManager.Models
{
    public class ResponseParameters
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixGeneralAnalyzer.DiagnosticId).WithSpan(3, 18, 3, 36).WithArguments("ResponseParameters", "Parameters", "'ResponseContent' or 'ResponsePatch'");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ResultSuffix()
        {
            var test = @"namespace Azure.ResourceManager.Models
{
    public class NetworkRequest
    {
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixGeneralAnalyzer.DiagnosticId).WithSpan(3, 18, 3, 32).WithArguments("NetworkRequest", "Request", "'NetworkContent'");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task OptionSuffixWithNestedNameSpace()
        {
            var test = @"namespace Azure.ResourceManager.Models
{
    namespace SubTest
    {
        public class DiskOption
        {
        }
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixGeneralAnalyzer.DiagnosticId).WithSpan(5, 22, 5, 32).WithArguments("DiskOption", "Option", "'DiskConfig'");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ResponsesSuffix()
        {
            var test = @"namespace Azure.ResourceManager.Models
{
    namespace SubTest
    {
        public class CreationResponses
        {
        }
    }
}";
            var expected = VerifyCS.Diagnostic(ModelNameSuffixGeneralAnalyzer.DiagnosticId).WithSpan(5, 22, 5, 39).WithArguments("CreationResponses", "Responses", "'CreationResults'");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
