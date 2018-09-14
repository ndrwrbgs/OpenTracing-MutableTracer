# Keeping the multiple tracer state

## Option 1 - ITracer ExchangeTracer(Func<ITracer> wrapExisting)

Pro - ease of implementing THIS package
Con - users must ensure they actually wrap the tracer, or we'd lose messages
Con - the 'new' tracer will come ABOVE the previous tracer
	Pro - Allows a filtering style tracer, that does not send to lower levels

## Option 2 - Stack<ITracer>

Pro - Allows a kind of chain-of-command pattern
Con - Requires maintaining AsyncLocal state of all the tracer. Unless we can get a 'asynclocalchanged' event hook, we'd have to make a new Stack (certain optimizations omitted, for brevity) for each async
Pro - Allows the last tracer added to be called last (e.g. you can't modify your callers' intentions to trace with your own non-pass-through)
Pro - Works with existing tracer implementations that are not wrap-ready