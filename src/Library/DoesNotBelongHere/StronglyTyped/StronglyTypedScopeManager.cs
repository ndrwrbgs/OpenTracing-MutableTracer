namespace OpenTracing.Contrib.MutableTracer
{
    public abstract class StronglyTypedScopeManager<TScope, TSpan> : IScopeManager
        where TScope : IScope
        where TSpan : ISpan
    {
        public abstract TScope Activate(TSpan span, bool finishSpanOnDispose);
        public abstract TScope Active { get; }
        IScope IScopeManager.Activate(ISpan span, bool finishSpanOnDispose)
        {
            return this.Activate((TSpan)span, finishSpanOnDispose);
        }

        IScope IScopeManager.Active => this.Active;
    }
}