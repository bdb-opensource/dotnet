namespace StackExchange.Profiling.Wcf
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;

    /// <summary>
    /// The WCF mini profiler client inspector.
    /// </summary>
    public class WcfMiniProfilerClientInspector : IClientMessageInspector
    {
        /// <summary>
        /// true if the binding is using http.
        /// </summary>
        private bool _http;

        /// <summary>
        /// before the send request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>the mini profiler start</returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var miniProfiler = MiniProfiler.Current;
            if (miniProfiler == null)
            {
                return null;
            }

            miniProfiler.Step($"WCF call to {channel.RemoteAddress.Uri}");

            var header = new MiniProfilerRequestHeader
            {
                User = miniProfiler.User,
                ParentProfilerId = miniProfiler.Id
            };

            // ReSharper disable PossibleUnintendedReferenceComparison
            if (request.Headers.MessageVersion != MessageVersion.None)
            // ReSharper restore PossibleUnintendedReferenceComparison
            {
                var untypedHeader = new MessageHeader<MiniProfilerRequestHeader>(header)
                    .GetUntypedHeader(MiniProfilerRequestHeader.HeaderName, MiniProfilerRequestHeader.HeaderNamespace);
                request.Headers.Add(untypedHeader);
            }
            else if (_http || WebOperationContext.Current != null || channel.Via.Scheme == "http" || channel.Via.Scheme == "https")
            {
                _http = true;

                object property;
                if (!request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out property))
                {
                    property = new HttpRequestMessageProperty();
                    request.Properties.Add(HttpRequestMessageProperty.Name, property);
                }
                ((HttpRequestMessageProperty)property).Headers.Add(MiniProfilerRequestHeader.HeaderName, header.ToHeaderText());
            }
            else
            {
                throw new InvalidOperationException("MVC Mini Profiler does not support EnvelopeNone unless HTTP is the transport mechanism");
            }

            return new MiniProfilerState
            {
                Timing = miniProfiler.Head,
                // Can't use MiniProfiler.DurationMilliseconds as it is set only when the profiler is stopped
                StartTime = miniProfiler.GetElapsedMilliseconds()
            };
        }

        /// <summary>
        /// after the reply is received.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="correlationState">The correlation state.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var profilerState = correlationState as MiniProfilerState;

            if (profilerState == null || profilerState.Timing == null)
            {
                return;
            }
            // Check to see if we have a request as part of this message
            MiniProfilerResultsHeader resultsHeader = null;
            // ReSharper disable PossibleUnintendedReferenceComparison
            if (reply.Headers.MessageVersion != MessageVersion.None)
            // ReSharper restore PossibleUnintendedReferenceComparison
            {
                var headerIndex = reply.Headers.FindHeader(MiniProfilerResultsHeader.HeaderName, MiniProfilerResultsHeader.HeaderNamespace);
                if (headerIndex >= 0)
                {
                    resultsHeader = reply.Headers.GetHeader<MiniProfilerResultsHeader>(headerIndex);
                }
            }
            else if (_http || reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
            {
                _http = true;

                var property = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];

                var text = property.Headers[MiniProfilerResultsHeader.HeaderName];
                if (!string.IsNullOrEmpty(text))
                {
                    resultsHeader = MiniProfilerResultsHeader.FromHeaderText(text);
                }
            }
            else
            {
                throw new InvalidOperationException("MVC Mini Profiler does not support EnvelopeNone unless HTTP is the transport mechanism");
            }

            if (resultsHeader == null || resultsHeader.ProfilerResults == null)
            {
                return;
            }

            resultsHeader.ProfilerResults.Root.UpdateStartMillisecondTimingsToAbsolute(profilerState.StartTime);
            profilerState.Timing.AddChild(resultsHeader.ProfilerResults.Root);

            profilerState.Timing.Stop();
        }

        /// <summary>
        /// The mini profiler state before the WCF call.
        /// </summary>
        private class MiniProfilerState
        {
            /// <summary>
            /// Gets or sets the timing within which the WCF call was made.
            /// </summary>
            public Timing Timing { get; set; }

            /// <summary>
            /// Gets or sets the number of miliseconds between the start of profiler and the beginning of the WCF call.
            /// </summary>
            public decimal StartTime { get; set; }
        }
    }
}