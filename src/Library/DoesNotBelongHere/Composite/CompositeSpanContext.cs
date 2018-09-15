namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CompositeSpanContext : StronglyTypedSpanContext
    {
        internal ISpanContext[] SpanContexts { get; }

        public CompositeSpanContext(IEnumerable<ISpanContext> spanContexts)
        {
            this.SpanContexts = spanContexts.ToArray();
        }

        public override IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
        {
            return this.SpanContexts
                .SelectMany(context => context.GetBaggageItems())
                .Distinct();
        }

        public override string TraceId
        {
            get
            {
                // TODO: Not an accurate representation, but impossible to give a SINGLE string as we can't know which tracer it's wanting that string from
                // First tracer seems close enough
                return this.SpanContexts
                    .Select(span => span.TraceId)
                    .FirstOrDefault(traceId => traceId != null);
            }
        }

        public override string SpanId
        {
            get
            {
                // TODO: Not an accurate representation, but impossible to give a SINGLE string as we can't know which tracer it's wanting that string from
                // First tracer seems close enough
                return this.SpanContexts
                    .Select(span => span.SpanId)
                    .FirstOrDefault(spanId => spanId != null);
            }
        }
    }
}