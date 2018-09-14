using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using OpenTracing;
    using OpenTracing.Contrib.MutableTracer;
    using OpenTracing.Propagation;
    using OpenTracing.Util;

    class TDD
    {
        public void TestingIt()
        {
            MutableGlobalTracer.Initialize(globalTracer: new FakeConsoleTracer());

            using (GlobalTracer.Instance
                .BuildSpan("Overall")
                .StartActive())
            {
                // Op 1
                var operation1 = Task.Run(
                    async () =>
                    {
                        using (MutableGlobalTracer.UseTracer(new FakeFileTracer("Operation1 Path")))
                        {
                            using (GlobalTracer.Instance
                                .BuildSpan("Operation1")
                                .StartActive())
                            {
                            }
                        }
                    });

                // Op 2
                var operation2 = Task.Run(
                    async () =>
                    {
                        using (MutableGlobalTracer.UsingTracer(new FakeFileTracer("Operation2 Path"))
                            .BuildSpan("Operation2")
                            .StartActive())
                        {
                        }
                    });
            }
        }

        private sealed class FakeConsoleTracer : ITracer
        {
            public ISpanBuilder BuildSpan(string operationName)
            {
                throw new NotImplementedException();
            }

            public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
            {
                throw new NotImplementedException();
            }

            public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
            {
                throw new NotImplementedException();
            }

            public IScopeManager ScopeManager { get; }
            public ISpan ActiveSpan { get; }
        }

        private sealed class FakeFileTracer : ITracer
        {
            private string v;

            public FakeFileTracer(string v)
            {
                this.v = v;
            }

            public ISpanBuilder BuildSpan(string operationName)
            {
                throw new NotImplementedException();
            }

            public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
            {
                throw new NotImplementedException();
            }

            public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
            {
                throw new NotImplementedException();
            }

            public IScopeManager ScopeManager { get; }
            public ISpan ActiveSpan { get; }
        }
    }
}