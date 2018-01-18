using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange.Profiling.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="Timing"/>
    /// </summary>
    public static class TimingExtensions
    {
        /// <summary>
        /// Gets the start time of the <see cref="Timing"/>.
        /// </summary>
        /// <seealso cref="Timing.StartMilliseconds"/>
        public static DateTime? GetStartDateTime(this Timing timing)
        {
            // TODO: Profiler is not always defined. 
            return timing.Profiler?.Started.AddMilliseconds((double)timing.StartMilliseconds).ToLocalTime();
        }
    }
}