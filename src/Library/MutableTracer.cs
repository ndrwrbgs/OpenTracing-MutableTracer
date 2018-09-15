namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    [PublicAPI]
    public sealed class MutableTracer : AsyncLocalTracer<ICompositeTracer>
    {
        public MutableTracer(ITracer tracerImplementation)
        {
            this.Tracer.Value = CompositeTracerFactory.Create(new[] {tracerImplementation});
        }

        public IDisposable UseTracer(ITracer tracer)
        {
            // Locking might not be necessary due to being AsyncLocal
            var previousTracers = this.Tracer.Value.Tracers.ToArray();

            var newTracers = new List<ITracer>(previousTracers.Length + 1);
            newTracers.AddRange(previousTracers);
            newTracers.Add(tracer);

            this.Tracer.Value = CompositeTracerFactory.Create(newTracers);

            var expectedLastTracer = tracer;
            return Disposable.Create(
                () =>
                {
                    var currentTracers = this.Tracer.Value.Tracers.ToArray();
                    var lastTracer = currentTracers.Last();

                    if (lastTracer != expectedLastTracer)
                    {
                        throw new InvalidOperationException(
                            "Logic bug, trying to remove a tracer other than the one added. Check your Disposes.");
                    }

                    var newTracersCount = currentTracers.Length - 1;
                    var newTracers2 = new ITracer[newTracersCount];
                    Array.Copy(currentTracers, 0, newTracers2, 0, newTracersCount);
                    this.Tracer.Value = CompositeTracerFactory.Create(newTracers2);
                });
        }
    }
}