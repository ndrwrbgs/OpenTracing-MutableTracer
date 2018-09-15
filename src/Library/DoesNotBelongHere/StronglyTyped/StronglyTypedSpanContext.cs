namespace OpenTracing.Contrib.MutableTracer
{
    using System.Collections.Generic;

    /// <summary>
    /// Doesn't do anything now, included for ease in the future should that change, and for consistency for implementors.
    /// </summary>
    public abstract class StronglyTypedSpanContext : ISpanContext
    {
        public abstract IEnumerable<KeyValuePair<string, string>> GetBaggageItems();
        public abstract string TraceId { get; }
        public abstract string SpanId { get; }
    }
}