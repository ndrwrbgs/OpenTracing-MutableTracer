namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using OpenTracing.Tag;

    public abstract class StronglyTypedSpanBuilder<TThis, TSpanContext, TSpan, TScope> : ISpanBuilder
        where TThis : StronglyTypedSpanBuilder<TThis, TSpanContext, TSpan, TScope>
        where TSpanContext : ISpanContext
        where TSpan : ISpan
        where TScope : IScope
    {
        public abstract TThis AsChildOf(TSpanContext parent);

        public abstract TThis AsChildOf(TSpan parent);

        public abstract TThis AddReference(string referenceType, TSpanContext referencedContext);

        public abstract TThis IgnoreActiveSpan();

        public abstract TThis WithTag(string key, string value);

        public abstract TThis WithTag(string key, bool value);

        public abstract TThis WithTag(string key, int value);

        public abstract TThis WithTag(string key, double value);

        public abstract TThis WithTag(BooleanTag tag, bool value);

        public abstract TThis WithTag(IntOrStringTag tag, string value);

        public abstract TThis WithTag(IntTag tag, int value);

        public abstract TThis WithTag(StringTag tag, string value);

        public abstract TThis WithStartTimestamp(DateTimeOffset timestamp);

        public abstract TScope StartActive();

        public abstract TScope StartActive(bool finishSpanOnDispose);

        public abstract TSpan Start();

        ISpanBuilder ISpanBuilder.AsChildOf(ISpanContext parent)
        {
            return this.AsChildOf((TSpanContext) parent);
        }

        ISpanBuilder ISpanBuilder.AsChildOf(ISpan parent)
        {
            return this.AsChildOf((TSpan) parent);
        }

        ISpanBuilder ISpanBuilder.AddReference(string referenceType, ISpanContext referencedContext)
        {
            return this.AddReference(referenceType, (TSpanContext) referencedContext);
        }

        ISpanBuilder ISpanBuilder.IgnoreActiveSpan()
        {
            return this.IgnoreActiveSpan();
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, string value)
        {
            return this.WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, bool value)
        {
            return this.WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, int value)
        {
            return this.WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, double value)
        {
            return this.WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(BooleanTag tag, bool value)
        {
            return this.WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(IntOrStringTag tag, string value)
        {
            return this.WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(IntTag tag, int value)
        {
            return this.WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(StringTag tag, string value)
        {
            return this.WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithStartTimestamp(DateTimeOffset timestamp)
        {
            return this.WithStartTimestamp(timestamp);
        }

        IScope ISpanBuilder.StartActive()
        {
            return this.StartActive();
        }

        IScope ISpanBuilder.StartActive(bool finishSpanOnDispose)
        {
            return this.StartActive(finishSpanOnDispose);
        }

        ISpan ISpanBuilder.Start()
        {
            return this.Start();
        }
    }
}