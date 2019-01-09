[![NuGet](https://img.shields.io/nuget/v/OpenTracing.Contrib.MutableTracer.svg)](https://www.nuget.org/packages/OpenTracing.Contrib.MutableTracer)

# OpenTracing-MutableTracer
The OpenTracing C# API provides an immutable GlobalTracer. This library provides a tracer you can use to mutate the current `GlobalTracer.Instance` as required.

Example use case: You want to trace everything to your master trace output file, but sub-operations need to be traced to specific files in children directories (e.g. tracing a Build operation).

# Note
Generally, readers should prefer to set an AsyncLocal sink for the output - e.g. at the TraceListener level rather than at the GlobalTracer level.

## Example Usage
```Csharp
MutableGlobalTracer.Initialize(globalTracer: new FakeConsoleTracer("A"));

using (GlobalTracer.Instance
  .BuildSpan("Overall")
  .StartActive())
{
  var operation1 = Task.Run(
    async () =>
    {
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
    
  // Obviously this is a cutting from code I was testing with, sorry for the variable names :)
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

        // To make sure operation1 has also started before this finishes
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
```

* FakeConsoleTracer("A") will receive `Overall`, `Operation1.1`/`Operation3.1` and then `Operation1.2`/`Operation3.2`
* FakeConsoleTracer("B") will receive `Operation1.1` then `Operation1.2`
* FakeConsoleTracer("C") will receive `Operation3.1` then `Operation3.2`
