namespace OpenTracing.Contrib.MutableTracer
{
    using System;

    internal sealed class Disposable : IDisposable
    {
        private readonly Action onDispose;

        public Disposable(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public static IDisposable Create(Action onDispose)
        {
            return new Disposable(onDispose);
        }

        public void Dispose()
        {
            this.onDispose();
        }
    }
}