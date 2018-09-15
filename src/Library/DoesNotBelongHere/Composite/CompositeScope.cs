namespace OpenTracing.Contrib.MutableTracer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class CompositeScope : StronglyTypedScope<CompositeSpan>
    {
        internal IScope[] Scopes { get; }

        public CompositeScope(IEnumerable<IScope> scopes)
        {
            this.Scopes = scopes.ToArray();
        }

        public override void Dispose()
        {
            foreach (var scope in this.Scopes)
            {
                scope.Dispose();
            }
        }

        public override CompositeSpan Span
        {
            get
            {
                return new CompositeSpan(
                    this.Scopes
                        .Select(scope => scope.Span));
            }
        }
    }
}