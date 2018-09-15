namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CompositeScopeManager : StronglyTypedScopeManager<CompositeScope, CompositeSpan>
    {
        internal IScopeManager[] ScopeManagers { get; }

        public CompositeScopeManager(IEnumerable<IScopeManager> scopeManagers)
        {
            this.ScopeManagers = scopeManagers.ToArray();
        }

        public override CompositeScope Activate(CompositeSpan span, bool finishSpanOnDispose)
        {

            return new CompositeScope(
                this.ScopeManagers
                    .Select((scopeManager, index) =>
                    {
                        var spanForScope = span.Spans[index];
                        return scopeManager.Activate(spanForScope, finishSpanOnDispose);
                    }));
        }

        public override CompositeScope Active
        {
            get
            {
                return new CompositeScope(
                    this.ScopeManagers
                        .Select(scopeManager => scopeManager.Active));
            }
        }
    }
}