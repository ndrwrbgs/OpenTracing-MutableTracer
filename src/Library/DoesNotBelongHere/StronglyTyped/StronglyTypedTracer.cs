using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.MutableTracer
{
    using OpenTracing.Propagation;

    public abstract class StronglyTypedTracer<TSpanBuilder, TSpanContext, TScopeManager, TSpan> : ITracer
        where TSpanBuilder : ISpanBuilder
        where TSpan : ISpan
        where TScopeManager : IScopeManager
        where TSpanContext : ISpanContext
    {
        ISpanBuilder ITracer.BuildSpan(string operationName)
        {
            return this.BuildSpan(operationName);
        }

        public abstract TSpanBuilder BuildSpan(string operationName);

        void ITracer.Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            if (!(spanContext is TSpanContext))
            {
                throw new ArgumentException();
            }

            Inject((TSpanContext) spanContext, format, carrier);
        }

        public abstract void Inject<TCarrier>(TSpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier);

        ISpanContext ITracer.Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            return this.Extract(format, carrier);
        }

        public abstract TSpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier);

        IScopeManager ITracer.ScopeManager => this.ScopeManager;

        public abstract TScopeManager ScopeManager { get; }

        ISpan ITracer.ActiveSpan => this.ActiveSpan;

        public abstract TSpan ActiveSpan { get; }
    }
}