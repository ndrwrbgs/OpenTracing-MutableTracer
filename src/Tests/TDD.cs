using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using OpenTracing;
    using OpenTracing.Contrib.MutableTracer;
    using OpenTracing.Noop;
    using OpenTracing.Propagation;
    using OpenTracing.Util;
    using WpfApp1;

    [TestFixture]
    class TDD
    {
        [Test]
        public void TestingIt()
        {
            MutableGlobalTracer.Initialize(globalTracer: new FakeConsoleTracer("A"));
            TestDataGenerator.GenerateTestData().Wait();
            return;
            MutableGlobalTracer.Initialize(globalTracer: new FakeConsoleTracer("A"));

            using (GlobalTracer.Instance
                .BuildSpan("Overall")
                .StartActive())
            {
                // Op 1
                var operation1 = Task.Run(
                    async () =>
                    {
                        //using (MutableGlobalTracer.UseTracer(new FakeFileTracer("Operation1 Path")))
                        using (MutableGlobalTracer.UseTracer(new FakeConsoleTracer("B")))
                        {
                            using (GlobalTracer.Instance
                                .BuildSpan("Operation1.1")
                                .StartActive())
                            {
                            }

                            // To make sure operation3 has also started before this finishes
                            await Task.Delay(100);

                            using (GlobalTracer.Instance
                                .BuildSpan("Operation1.2")
                                .StartActive())
                            {
                            }
                        }
                    });

                // Op 2
                //var operation2 = Task.Run(
                //    async () =>
                //    {
                //        using (MutableGlobalTracer.UsingTracer(new FakeFileTracer("Operation2 Path"))
                //            .BuildSpan("Operation2")
                //            .StartActive())
                //        {
                //        }
                //    });

                // Op 3
                var operation3 = Task.Run(
                    async () =>
                    {
                        using (MutableGlobalTracer.UseTracer(new FakeConsoleTracer("C")))
                        {
                            using (GlobalTracer.Instance
                                .BuildSpan("Operation3.1")
                                .StartActive())
                            {
                            }

                            // To make sure operation3 has also started before this finishes
                            await Task.Delay(100);

                            using (GlobalTracer.Instance
                                .BuildSpan("Operation3.2")
                                .StartActive())
                            {
                            }
                        }
                    });

                operation1.Wait();
                operation3.Wait();
            }
        }

        private sealed class FakeConsoleTracer : ITracer
        {
            private readonly ITracer noop = NoopTracerFactory.Create();
            private readonly string prefix;

            public FakeConsoleTracer(string prefix)
            {
                this.prefix = prefix;
            }

            public ISpanBuilder BuildSpan(string operationName)
            {
                Console.WriteLine($"{this.prefix} Building a span {operationName}");
                return this.noop.BuildSpan(operationName);
            }

            public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
            {
            }

            public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
            {
                return this.noop.Extract(format, carrier);
            }

            public IScopeManager ScopeManager => this.noop.ScopeManager;
            public ISpan ActiveSpan => this.noop.ActiveSpan;
        }

        private sealed class FakeFileTracer : ITracer
        {
            private string v;

            private readonly ITracer noop = NoopTracerFactory.Create();

            public FakeFileTracer(string v)
            {
                this.v = v;
            }

            public ISpanBuilder BuildSpan(string operationName)
            {
                return this.noop.BuildSpan(operationName);
            }

            public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
            {
                this.noop.Inject(spanContext, format, carrier);
            }

            public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
            {
                return this.noop.Extract(format, carrier);
            }

            public IScopeManager ScopeManager => this.noop.ScopeManager;
            public ISpan ActiveSpan => this.noop.ActiveSpan;
        }
    }
}