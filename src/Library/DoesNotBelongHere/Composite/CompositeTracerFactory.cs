namespace OpenTracing.Contrib.MutableTracer
{
    using System.Collections;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    /// <summary>
    /// CompositeTracer is internal because to use StronglyTypedTracer it must have the SpanBuilder etc as exposed at the same
    /// visibility as itself. As we generally do not want to expose those, we wrap up it's ctor here.
    /// </summary>
    [PublicAPI]
    public static class CompositeTracerFactory
    {
        public static ICompositeTracer Create(IEnumerable<ITracer> tracers)
        {
            return new CompositeTracer(tracers);
        }
    }
}