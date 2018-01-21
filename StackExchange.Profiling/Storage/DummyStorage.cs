using System;
using System.Collections.Generic;

namespace StackExchange.Profiling.Storage
{
    /// <summary>
    /// A dummy storage which doesn't actually store the profilers anywhere.
    /// </summary>
    public class DummyStorage : IStorage
    {
        /// <summary>
        /// Returns an empty list.
        /// </summary>
        public List<Guid> GetUnviewedIds(string user)
        {
            return new List<Guid>();
        }

        /// <summary>
        /// Returns an empty list.
        /// </summary>
        public IEnumerable<Guid> List(int maxResults, DateTime? start = null, DateTime? finish = null, ListResultsOrder orderBy = ListResultsOrder.Descending)
        {
            return new List<Guid>();
        }

        /// <summary>
        /// Returns null.
        /// </summary>
        public MiniProfiler Load(Guid id)
        {
            return null;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Save(MiniProfiler profiler)
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void SetUnviewed(string user, Guid id)
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void SetViewed(string user, Guid id)
        {
        }
    }
}