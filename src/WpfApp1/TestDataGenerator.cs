namespace WpfApp1
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using OpenTracing.Util;

    internal static class TestDataGenerator
    {
        public static async Task GenerateTestData()
        {
            var operation1 = Task.Run(
                async () =>
                {
                    using (GlobalTracer.Instance
                        .BuildSpan("Operation 1")
                        .StartActive())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            using (GlobalTracer.Instance.BuildSpan("Nested").StartActive())
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Console.WriteLine($"Simulate iteration {i} - pause for 100ms");
                                    await Task.Delay(100);
                                }
                            }
                        }
                    }
                });
            // Make sure 1 starts first
            await Task.Delay(10);

            var operation2 = Task.Run(
                async () =>
                {
                    using (GlobalTracer.Instance
                        .BuildSpan("Operation 2")
                        .StartActive())
                    {
                        Task sub1;
                        // TODO: Noticed a problem with the OT API and async/await. An unobserved task will capture it's AsyncLocal parent,
                        // but that parent can easily be closed while that async task is still running. Oops :-S
                        sub1 = Task.Run(
                            async () =>
                            {
                                using (GlobalTracer.Instance.BuildSpan("SubOperation1").StartActive())
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        Console.WriteLine($"Simulate iteration {i} - pause for 7s");
                                        Console.WriteLine("Pausing for 7s");
                                        await Task.Delay(7000);
                                    }
                                }
                            });
                        // Make sure 1 starts first
                        await Task.Delay(10);
                        Task sub2 = Task.Run(
                            async () =>
                            {
                                using (GlobalTracer.Instance.BuildSpan("SubOperation2").StartActive())
                                {
                                    for (int i = 0; i < 15; i++)
                                    {
                                        Trace.WriteLine($"Simulate iteration {i} - pause for 1s");
                                        await Task.Delay(1000);
                                    }
                                }
                            });

                        await sub1;
                        await sub2;
                    }
                });

            await operation1;
            await operation2;
        }
    }
}