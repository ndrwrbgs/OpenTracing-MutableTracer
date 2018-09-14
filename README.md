# OpenTracing-MutableTracer
The OpenTracing C# API provides an immutable GlobalTracer. This library provides a tracer you can use to mutate the current `GlobalTracer.Instance` as required.

Example use case: You want to trace everything to your master trace output file, but sub-operations need to be traced to specific files in children directories (e.g. tracing a Build operation).
