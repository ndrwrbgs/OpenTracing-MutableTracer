namespace OpenTracing.Contrib.MutableTracer
{
    using System.Collections.Generic;

    public interface ICompositeTracer : ITracer
    {
        // TODO: Consider exposing the array directly for perf
        IEnumerable<ITracer> Tracers { get; }
    }
}