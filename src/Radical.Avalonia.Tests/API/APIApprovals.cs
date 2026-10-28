using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiGenerator;
using System.Runtime.CompilerServices;

namespace Radical.Avalonia.Tests.API
{
    [TestClass]
    public class APIApprovals
    {
        [TestMethod]
        [TestCategory("APIApprovals")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        [UseReporter(typeof(DiffReporter))]
        public void Approve_API()
        {
            ApiGeneratorOptions options = new ApiGeneratorOptions()
            {
                AllowNamespacePrefixes = ["Microsoft", "System"],
                ExcludeAttributes = ["System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute"]
            };

            var publicApi = typeof(RadicalApplication).Assembly.GeneratePublicApi(options: options);

            Approvals.Verify(publicApi);
        }
    }
}