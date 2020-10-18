using System.Collections.Generic;

namespace XLang.Parser.Base
{
    public class XLangBroadNameLookup<T>
    {

        private readonly Dictionary<string, T> resolvables;
        public int ResolvedCount { get; private set; }

        public XLangBroadNameLookup(Dictionary<string, T> resolvables)
        {
            this.resolvables = resolvables;
            EndRound();
        }
        public XLangBroadNameLookup() : this(new Dictionary<string, T>()) { }


        public bool CanResolve(string name)
        {
            return resolvables.ContainsKey(name);
        }

        public void AddResolved(string name, T item)
        {
            ResolvedCount++;
            resolvables.Add(name, item);
        }

        public T Resolve(string name)
        {
            return resolvables[name];
        }

        public void EndRound()
        {
            ResolvedCount = 0;
        }

    }
}