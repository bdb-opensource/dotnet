using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using NUnit.Framework;
using StackExchange.Profiling.Wcf;

namespace StackExchange.Profiling.Tests.WCF
{
    [TestFixture]
    public class WcfMiniProfilerDispatchInspectorTest
    {
        [Test]
        public void WillSuppressSQlCommandTextInHeader()
        {
            MiniProfiler.Settings.ProfilerProvider = new SingletonProfilerProvider();
            var profiler = MiniProfiler.Start(nameof(this.WillSuppressSQlCommandTextInHeader));
            profiler.Root.AddCustomTiming("sql",
                new CustomTiming(profiler, nameof(this.WillSuppressSQlCommandTextInHeader), 5));

            var message = Message.CreateMessage(MessageVersion.Soap12, nameof(this.WillSuppressSQlCommandTextInHeader));

            var wcfMiniProfilerDispatchInspector = new WcfMiniProfilerDispatchInspector(false);
            wcfMiniProfilerDispatchInspector.BeforeSendReply(ref message, new MiniProfilerRequestHeader());

            var resultHeader = message.Headers.GetHeader<MiniProfilerResultsHeader>(MiniProfilerResultsHeader.HeaderName,
                MiniProfilerResultsHeader.HeaderNamespace);
            Assert.AreEqual(0, resultHeader.ProfilerResults.Root.CustomTimings.Count());
        }

        [Test]
        public void WillSerializeProfilerWithoutSqlCommandText()
        {
            MiniProfiler.Settings.ProfilerProvider = new SingletonProfilerProvider();
            var profiler = MiniProfiler.Start(nameof(this.WillSuppressSQlCommandTextInHeader));

            var message = Message.CreateMessage(MessageVersion.Soap12, nameof(this.WillSuppressSQlCommandTextInHeader));

            var wcfMiniProfilerDispatchInspector = new WcfMiniProfilerDispatchInspector(false);
            wcfMiniProfilerDispatchInspector.BeforeSendReply(ref message, new MiniProfilerRequestHeader());

            var resultHeader = message.Headers.GetHeader<MiniProfilerResultsHeader>(MiniProfilerResultsHeader.HeaderName,
                MiniProfilerResultsHeader.HeaderNamespace);
            Assert.IsNull(resultHeader.ProfilerResults.Root.CustomTimings);
        }
    }
}