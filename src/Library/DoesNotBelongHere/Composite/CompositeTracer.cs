using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.MutableTracer
{
    using System.Collections.Generic;
    using OpenTracing.Propagation;

    internal sealed class CompositeTracer : StronglyTypedTracer<CompositeSpanBuilder, CompositeSpanContext, CompositeScopeManager, CompositeSpan>, ICompositeTracer
    {
        // TODO: Since these are all read only, we could cache things like ScopeManager and avoid re-allocations.
        internal ITracer[] Tracers { get; }

        IEnumerable<ITracer> ICompositeTracer.Tracers => this.Tracers;

        public CompositeTracer(IEnumerable<ITracer> tracers)
        {
            this.Tracers = tracers.ToArray();
        }

        public override CompositeSpanBuilder BuildSpan(string operationName)
        {
            return new CompositeSpanBuilder(
                this.Tracers
                    .Select(tracer => tracer.BuildSpan(operationName)));
        }

        public override CompositeSpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            return new CompositeSpanContext(
                this.Tracers
                    .Select(tracer => tracer.Extract(format, carrier)));
        }

        public override CompositeScopeManager ScopeManager
        {
            get
            {
                return new CompositeScopeManager(
                    this.Tracers
                        .Select(tracer => tracer.ScopeManager));
            }
        }

        public override CompositeSpan ActiveSpan
        {
            get
            {
                return new CompositeSpan(
                    this.Tracers
                        .Select(tracer => tracer.ActiveSpan));
            }
        }

        public override void Inject<TCarrier>(CompositeSpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            for (var index = 0; index < this.Tracers.Length; index++)
            {
                var tracer = this.Tracers[index];
                var contextForTracer = spanContext.SpanContexts[index];

                tracer.Inject(contextForTracer, format, carrier);
            }
        }
    }
}