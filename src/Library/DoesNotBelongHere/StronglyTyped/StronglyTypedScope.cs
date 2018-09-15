namespace OpenTracing.Contrib.MutableTracer
{
    using System;

    public abstract class StronglyTypedScope<TSpan> : IScope
        where TSpan : ISpan
    {
        public abstract void Dispose();
        public abstract TSpan Span { get; }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        ISpan IScope.Span => this.Span;
    }
}