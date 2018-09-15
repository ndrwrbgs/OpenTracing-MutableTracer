namespace OpenTracing.Contrib.MutableTracer
{
    using System.Threading;
    using OpenTracing.Propagation;

    public abstract class AsyncLocalTracer<TType> : ITracer
        where TType : ITracer
    {
        // TODO: Should be protected, but AsyncLocal is internal. Will need #if directives to address this for .NET45 vs 46
        internal AsyncLocal<TType> Tracer { get; } = new AsyncLocal<TType>();

        public ISpanBuilder BuildSpan(string operationName)
        {
            return this.Tracer.Value.BuildSpan(operationName);
        }

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            this.Tracer.Value.Inject(spanContext, format, carrier);
        }

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            return this.Tracer.Value.Extract(format, carrier);
        }

        public IScopeManager ScopeManager => this.Tracer.Value.ScopeManager;

        public ISpan ActiveSpan => this.Tracer.Value.ActiveSpan;
    }
}