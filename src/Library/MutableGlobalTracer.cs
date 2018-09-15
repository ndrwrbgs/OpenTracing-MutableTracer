using System;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.MutableTracer
{
    using System.Reflection;
    using JetBrains.Annotations;
    using OpenTracing.Tag;
    using OpenTracing.Util;

    /// <summary>
    /// Utility for tying <see cref="MutableTracer"/> to <see cref="GlobalTracer.Instance"/> and
    /// exposing static/cross-cutting-concern mutation methods that don't require tracking the instance.
    /// </summary>
    [PublicAPI]
    public static class MutableGlobalTracer
    {
        /// <remarks>
        /// Considered using the static ctor, but decided we shouldn't set static global state <see cref="GlobalTracer.Instance"/>
        /// without being explicitly told to do so.
        /// </remarks>
        public static void Initialize(ITracer globalTracer = null)
        {
            /* This will always be NoopTracer, since if it's anything else we cannot use GlobalTracer.Register.
             * but we defer to GlobalTracer for what it wants this Instance to be rather than propagating that
             * default into this code.
             */
            var tracerImplementation = globalTracer ?? GlobalTracer.Instance;

            var mutableTracer = new MutableTracer(tracerImplementation);
            GlobalTracer.Register(mutableTracer);
        }

        #region The Set hooks for MutableTracer

        // TODO: Expose specific struct Disposable type to make more efficient
        public static IDisposable UseTracer(ITracer tracer)
        {
            // TODO: Why on earth does GlobalTracer expose the ScopeManager, Span, SpanBuilder, but NOT the Tracer?

            // TODO: Perf on this reflection, cache and compile
            var currentTracer = typeof(GlobalTracer).GetField("_tracer", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(GlobalTracer.Instance);
            return (currentTracer as MutableTracer)
                .UseTracer(tracer);
        }

        /// <summary>
        /// Convenience method given that calls to <see cref="UseTracer"/> will typically correspond
        /// to a logical operation starting and be immediately followed by <see cref="ITracer.BuildSpan"/>.
        /// .
        /// This method just groups those two calls together.
        /// .
        /// The returned ITracer will call UseTracer when building any spans and will call Dispose on the UseTracer
        /// result when those spans are closed.
        /// </summary>
        internal static ITracer UsingTracer(ITracer tracer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}