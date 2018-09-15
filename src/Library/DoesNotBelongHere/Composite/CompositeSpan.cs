namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTracing.Tag;

    internal class CompositeSpan : StronglyTypedSpan<CompositeSpan, CompositeSpanContext>
    {
        internal ISpan[] Spans { get; }

        public CompositeSpan(IEnumerable<ISpan> spans)
        {
            this.Spans = spans.ToArray();
        }

        public override CompositeSpan SetTag(string key, string value)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetTag(key, value)));
        }

        public override CompositeSpan SetTag(string key, bool value)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetTag(key, value)));
        }

        public override CompositeSpan SetTag(string key, int value)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetTag(key, value)));
        }

        public override CompositeSpan SetTag(string key, double value)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetTag(key, value)));
        }

        public override CompositeSpan SetTag(BooleanTag tag, bool value)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetTag(tag, value)));
        }

        public override CompositeSpan SetTag(IntOrStringTag tag, string value)
        {
            throw new NotImplementedException();
        }

        public override CompositeSpan SetTag(IntTag tag, int value)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetTag(tag, value)));
        }

        public override CompositeSpan SetTag(StringTag tag, string value)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetTag(tag, value)));
        }

        public override CompositeSpan Log(IEnumerable<KeyValuePair<string, object>> fields)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.Log(fields)));
        }

        public override CompositeSpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.Log(timestamp, fields)));
        }

        public override CompositeSpan Log(string @event)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.Log(@event)));
        }

        public override CompositeSpan Log(DateTimeOffset timestamp, string @event)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.Log(timestamp, @event)));
        }

        public override CompositeSpan SetBaggageItem(string key, string value)
        {
            return new CompositeSpan(
                this.Spans
                    // TODO: If the code stays like this, would need FastLinq for Arrays to make it performant
                    .Select(span => span.SetBaggageItem(key, value)));
        }

        public override string GetBaggageItem(string key)
        {
            // TODO: Not an accurate representation, but impossible to give a SINGLE string as we can't know which tracer it's wanting that string from
            // this method is rather tracer specific anyway, and in non-tracer specific uses the First will suffice.
            return this.Spans
                .Select(span => span.GetBaggageItem(key))
                .FirstOrDefault(baggageItem => baggageItem != null);
        }

        public override CompositeSpan SetOperationName(string operationName)
        {
            return new CompositeSpan(
                this.Spans
                    .Select(span => span.SetOperationName(operationName)));
        }

        public override void Finish()
        {
            foreach (var span in this.Spans)
            {
                span.Finish();
            }
        }

        public override void Finish(DateTimeOffset finishTimestamp)
        {
            foreach (var span in this.Spans)
            {
                span.Finish(finishTimestamp);
            }
        }

        public override CompositeSpanContext Context
        {
            get
            {
                return new CompositeSpanContext(
                    this.Spans
                        .Select(span => span.Context));
            }
        }
    }
}