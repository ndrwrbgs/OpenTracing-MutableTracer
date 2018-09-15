namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using System.Collections.Generic;
    using OpenTracing.Tag;

    public abstract class StronglyTypedSpan<TThis, TSpanContext> : ISpan
        where TThis : StronglyTypedSpan<TThis, TSpanContext>
        where TSpanContext : ISpanContext
    {
        public abstract TThis SetTag(string key, string value);
        public abstract TThis SetTag(string key, bool value);
        public abstract TThis SetTag(string key, int value);
        public abstract TThis SetTag(string key, double value);
        public abstract TThis SetTag(BooleanTag tag, bool value);
        public abstract TThis SetTag(IntOrStringTag tag, string value);
        public abstract TThis SetTag(IntTag tag, int value);
        public abstract TThis SetTag(StringTag tag, string value);
        public abstract TThis Log(IEnumerable<KeyValuePair<string, object>> fields);
        public abstract TThis Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields);
        public abstract TThis Log(string @event);
        public abstract TThis Log(DateTimeOffset timestamp, string @event);
        public abstract TThis SetBaggageItem(string key, string value);
        public abstract string GetBaggageItem(string key);
        public abstract TThis SetOperationName(string operationName);
        public abstract void Finish();
        public abstract void Finish(DateTimeOffset finishTimestamp);
        public abstract TSpanContext Context { get; }

        ISpan ISpan.SetTag(string key, string value)
        {
            return this.SetTag(key, value);
        }

        ISpan ISpan.SetTag(string key, bool value)
        {
            return this.SetTag(key, value);
        }

        ISpan ISpan.SetTag(string key, int value)
        {
            return this.SetTag(key, value);
        }

        ISpan ISpan.SetTag(string key, double value)
        {
            return this.SetTag(key, value);
        }

        ISpan ISpan.SetTag(BooleanTag tag, bool value)
        {
            return this.SetTag(tag, value);
        }

        ISpan ISpan.SetTag(IntOrStringTag tag, string value)
        {
            return this.SetTag(tag, value);
        }

        ISpan ISpan.SetTag(IntTag tag, int value)
        {
            return this.SetTag(tag, value);
        }

        ISpan ISpan.SetTag(StringTag tag, string value)
        {
            return this.SetTag(tag, value);
        }

        ISpan ISpan.Log(IEnumerable<KeyValuePair<string, object>> fields)
        {
            return this.Log(fields);
        }

        ISpan ISpan.Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            return this.Log(timestamp, fields);
        }

        ISpan ISpan.Log(string @event)
        {
            return this.Log(@event);
        }

        ISpan ISpan.Log(DateTimeOffset timestamp, string @event)
        {
            return this.Log(timestamp, @event);
        }

        ISpan ISpan.SetBaggageItem(string key, string value)
        {
            return this.SetBaggageItem(key, value);
        }

        string ISpan.GetBaggageItem(string key)
        {
            return this.GetBaggageItem(key);
        }

        ISpan ISpan.SetOperationName(string operationName)
        {
            return this.SetOperationName(operationName);
        }

        void ISpan.Finish()
        {
            this.Finish();
        }

        // TODO: Remove the overrides that were generated that do not require special types
        void ISpan.Finish(DateTimeOffset finishTimestamp)
        {
            this.Finish(finishTimestamp);
        }

        ISpanContext ISpan.Context => this.Context;
    }
}