namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTracing.Tag;

    internal class CompositeSpanBuilder : StronglyTypedSpanBuilder<CompositeSpanBuilder, CompositeSpanContext, CompositeSpan, CompositeScope>
    {
        internal ISpanBuilder[] SpanBuilders { get; }

        public CompositeSpanBuilder(IEnumerable<ISpanBuilder> spanBuilders)
        {
            this.SpanBuilders = spanBuilders.ToArray();
        }

        public override CompositeSpanBuilder AsChildOf(CompositeSpanContext parent)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select((spanBuilder, index) =>
                    {
                        var contextForBuilder = parent.SpanContexts[index];
                        return spanBuilder.AsChildOf(contextForBuilder);
                    }));
        }

        public override CompositeSpanBuilder AsChildOf(CompositeSpan parent)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select((spanBuilder, index) =>
                    {
                        var spanForBuilder = parent.Spans[index];
                        return spanBuilder.AsChildOf(spanForBuilder);
                    }));
        }

        public override CompositeSpanBuilder AddReference(string referenceType, CompositeSpanContext referencedContext)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select((spanBuilder, index) =>
                    {
                        var contextForBuilder = referencedContext.SpanContexts[index];
                        return spanBuilder.AddReference(referenceType, contextForBuilder);
                    }));
        }

        public override CompositeSpanBuilder IgnoreActiveSpan()
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.IgnoreActiveSpan()));
        }

        public override CompositeSpanBuilder WithTag(string key, string value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(key, value)));
        }

        public override CompositeSpanBuilder WithTag(string key, bool value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(key, value)));
        }

        public override CompositeSpanBuilder WithTag(string key, int value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(key, value)));
        }

        public override CompositeSpanBuilder WithTag(string key, double value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(key, value)));
        }

        public override CompositeSpanBuilder WithTag(BooleanTag tag, bool value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value)));
        }

        public override CompositeSpanBuilder WithTag(IntOrStringTag tag, string value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value)));
        }

        public override CompositeSpanBuilder WithTag(IntTag tag, int value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value)));
        }

        public override CompositeSpanBuilder WithTag(StringTag tag, string value)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value)));
        }

        public override CompositeSpanBuilder WithStartTimestamp(DateTimeOffset timestamp)
        {
            return new CompositeSpanBuilder(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.WithStartTimestamp(timestamp)));
        }

        public override CompositeScope StartActive()
        {
            return new CompositeScope(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.StartActive()));
        }

        public override CompositeScope StartActive(bool finishSpanOnDispose)
        {
            return new CompositeScope(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.StartActive(finishSpanOnDispose)));
        }

        public override CompositeSpan Start()
        {
            return new CompositeSpan(
                this.SpanBuilders
                    .Select(spanBuilder => spanBuilder.Start()));
        }
    }
}