using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class ResolveCollection
    {
        private List<(MemberReference? ResolvableReference, IEnumerable<IResolvableDependencies?>? ResolvableDependencies)> memberReferences;

        public ResolveCollection() =>
            memberReferences = new List<(MemberReference?, IEnumerable<IResolvableDependencies?>?)>();

        public ResolveCollection IncludeDependencies(IEnumerable<IResolvableDependencies> references)
        {
            memberReferences.Add((null, references));
            return this;
        }

        public ResolveCollection IncludeDependencies(params IResolvableDependencies?[] references)
        {
            IncludeDependencies((IEnumerable<IResolvableDependencies>)references);
            return this;
        }

        public ResolveCollection IncludeReference(MemberReference reference)
        {
            memberReferences.Add((reference, null));
            return this;
        }

        public ResolveCollection Resolve() {
            foreach (var (memberReference, dependencies) in memberReferences) {
                if (!(dependencies is null)) {
                    foreach (var dep in dependencies) {
                        dep?.ResolveDependencies();
                    }
                }

                memberReference?.Resolve();
            }

            return this;
        }
    }

    public class ResolveCollection<T> : ResolveCollection
    {
        private T memberReference;

        public ResolveCollection(T memberReference) =>
            this.memberReference = memberReference;

        public new ResolveCollection IncludeDependencies(IEnumerable<IResolvableDependencies> references)
        {
            base.IncludeDependencies(references);
            return this;
        }

        public new ResolveCollection IncludeDependencies(params IResolvableDependencies?[] references)
        {
            base.IncludeDependencies((IEnumerable<IResolvableDependencies>)references);
            return this;
        }

        public new ResolveCollection IncludeReference(MemberReference reference)
        {
            base.IncludeDependencies(reference);
            return this;
        }

        public new T Resolve() {
            base.Resolve();
            return memberReference;
        }
    }
}
