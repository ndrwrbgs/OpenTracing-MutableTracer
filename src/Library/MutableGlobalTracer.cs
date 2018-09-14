using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.MutableTracer
{
    using System.Threading;
    using JetBrains.Annotations;
    using OpenTracing.Propagation;
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
            return (GlobalTracer.Instance as MutableTracer)
                .UseTracer(tracer);
        }

        /// <summary>
        /// Convenience method given that calls to <see cref="UseTracer"/> will typically correspond
        /// to a logical operation starting and be immediately followed by <see cref="ITracer.BuildSpan"/>.
        /// .
        /// This method just groups those two calls together.
        /// </summary>
        internal static ITracer UsingTracer(ITracer tracer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [PublicAPI]
    public sealed class MutableTracer : ITracer
    {
        /// <remarks>
        /// Note that we must have some object and not an array, as arrays are copy by value
        /// </remarks>
        private readonly AsyncLocal<List<ITracer>> tracerImplementation = new AsyncLocal<List<ITracer>>();

        public MutableTracer(ITracer tracerImplementation)
        {
            this.tracerImplementation.Value = new List<ITracer>(1)
            {
                tracerImplementation
            };
        }

        public IDisposable UseTracer(ITracer tracer)
        {
            // Locking might not be necessary due to being AsyncLocal
            var previousValue = this.tracerImplementation.Value;

            var newValue = new List<ITracer>(previousValue.Count + 1);
            newValue.AddRange(previousValue);
            newValue.Add(tracer);

            throw new NotImplementedException();
        }

        #region ITracer

        ISpanBuilder ITracer.BuildSpan(string operationName)
        {
            return this.tracerImplementation.BuildSpan(operationName);
        }

        void ITracer.Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            this.tracerImplementation.Inject(spanContext, format, carrier);
        }

        ISpanContext ITracer.Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            return this.tracerImplementation.Extract(format, carrier);
        }

        IScopeManager ITracer.ScopeManager => this.tracerImplementation.ScopeManager;

        ISpan ITracer.ActiveSpan => this.tracerImplementation.ActiveSpan;

        #endregion
    }

    internal sealed class CompositeSpanBuilder : ISpanBuilder
    {
        private ISpanBuilder[] spanBuilderImplementation;

        public CompositeSpanBuilder(ISpanBuilder[] spanBuilderImplementation)
        {
            this.spanBuilderImplementation = spanBuilderImplementation;
        }

        public ISpanBuilder AsChildOf(ISpanContext parent)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.AsChildOf(parent))
                    .ToArray());
        }

        public ISpanBuilder AsChildOf(ISpan parent)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.AsChildOf(parent))
                    .ToArray());
        }

        public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.AddReference(referenceType, referencedContext))
                    .ToArray());
        }

        public ISpanBuilder IgnoreActiveSpan()
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.IgnoreActiveSpan())
                    .ToArray());
        }

        public ISpanBuilder WithTag(string key, string value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(key, value))
                    .ToArray());
        }

        public ISpanBuilder WithTag(string key, bool value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(key, value))
                    .ToArray());
        }

        public ISpanBuilder WithTag(string key, int value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(key, value))
                    .ToArray());
        }

        public ISpanBuilder WithTag(string key, double value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(key, value))
                    .ToArray());
        }

        public ISpanBuilder WithTag(BooleanTag tag, bool value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value))
                    .ToArray());
        }

        public ISpanBuilder WithTag(IntOrStringTag tag, string value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value))
                    .ToArray());
        }

        public ISpanBuilder WithTag(IntTag tag, int value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value))
                    .ToArray());
        }

        public ISpanBuilder WithTag(StringTag tag, string value)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithTag(tag, value))
                    .ToArray());
        }

        public ISpanBuilder WithStartTimestamp(DateTimeOffset timestamp)
        {
            return new CompositeSpanBuilder(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.WithStartTimestamp(timestamp))
                    .ToArray());
        }

        public IScope StartActive()
        {
            return new CompositeScope(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.StartActive())
                    .ToArray());
        }

        public IScope StartActive(bool finishSpanOnDispose)
        {
            return new CompositeScope(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.StartActive(finishSpanOnDispose))
                    .ToArray());
        }

        public ISpan Start()
        {
            return new CompositeSpan(
                this.spanBuilderImplementation
                    .Select(spanBuilder => spanBuilder.Start())
                    .ToArray());
        }
    }

    internal sealed class CompositeScope : IScope
    {
        private readonly IScope[] scopeImplementation;

        public CompositeScope(IScope[] scopeImplementation)
        {
            this.scopeImplementation = scopeImplementation;
        }

        public void Dispose()
        {
            foreach (var scope in this.scopeImplementation)
            {
                scope.Dispose();
            }
        }

        public ISpan Span
        {
            get
            {
                return new CompositeSpan(
                    this.scopeImplementation
                        .Select(scope => scope.Span)
                        .ToArray());
            }
        }
    }

    internal sealed class CompositeSpan : ISpan
    {
        private readonly ISpan[] spanImplementation;

        public CompositeSpan(ISpan[] spanImplementation)
        {
            this.spanImplementation = spanImplementation;
        }

        public ISpan SetTag(string key, string value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(key, value))
                    .ToArray());
        }

        public ISpan SetTag(string key, bool value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(key, value))
                    .ToArray());
        }

        public ISpan SetTag(string key, int value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(key, value))
                    .ToArray());
        }

        public ISpan SetTag(string key, double value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(key, value))
                    .ToArray());
        }

        public ISpan SetTag(BooleanTag tag, bool value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(tag, value))
                    .ToArray());
        }

        public ISpan SetTag(IntOrStringTag tag, string value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(tag, value))
                    .ToArray());
        }

        public ISpan SetTag(IntTag tag, int value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(tag, value))
                    .ToArray());
        }

        public ISpan SetTag(StringTag tag, string value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetTag(tag, value))
                    .ToArray());
        }

        public ISpan Log(IEnumerable<KeyValuePair<string, object>> fields)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.Log(fields))
                    .ToArray());
        }

        public ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.Log(timestamp, fields))
                    .ToArray());
        }

        public ISpan Log(string @event)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.Log(@event))
                    .ToArray());
        }

        public ISpan Log(DateTimeOffset timestamp, string @event)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.Log(timestamp, @event))
                    .ToArray());
        }

        public ISpan SetBaggageItem(string key, string value)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetBaggageItem(key, value))
                    .ToArray());
        }

        public string GetBaggageItem(string key)
        {
            // TODO: Not accurate representation of composite
            return this.spanImplementation[0].GetBaggageItem(key);
        }

        public ISpan SetOperationName(string operationName)
        {
            return new CompositeSpan(
                this.spanImplementation
                    .Select(span => span.SetOperationName(operationName))
                    .ToArray());
        }

        public void Finish()
        {
            foreach (var span in this.spanImplementation)
            {
                span.Finish();
            }
        }

        public void Finish(DateTimeOffset finishTimestamp)
        {
            foreach (var span in this.spanImplementation)
            {
                span.Finish(finishTimestamp);
            }
        }

        public ISpanContext Context
        {
            get
            {
                // TODO: Not accurate representation of composite
                // TODO: Won't actually work
                return this.spanImplementation[0].Context;
            }
        }
    }
}