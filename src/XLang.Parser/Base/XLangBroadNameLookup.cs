using System.Collections.Generic;

namespace XLang.Parser.Base
{
    /// <summary>
    ///     Name Lookup Helper Class that is used to keep track of Resolved Symbols
    /// </summary>
    /// <typeparam name="T">Type of Symbols</typeparam>
    public class XLangBroadNameLookup<T>
    {
        /// <summary>
        ///     Internal Type / Symbol Map
        /// </summary>
        private readonly Dictionary<string, T> resolvables;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="resolvables">Resolved Symbols</param>
        public XLangBroadNameLookup(Dictionary<string, T> resolvables)
        {
            this.resolvables = resolvables;
            EndCycle();
        }

        /// <summary>
        ///     Public Constructor
        /// </summary>
        public XLangBroadNameLookup() : this(new Dictionary<string, T>())
        {
        }

        /// <summary>
        ///     Resolved Items in the current resolve cycle
        /// </summary>
        public int ResolvedCount { get; private set; }

        /// <summary>
        ///     Returns true if the Name can be resolved to a Symbol
        /// </summary>
        /// <param name="name">Symbol Name</param>
        /// <returns>True if can Resolve</returns>
        public bool CanResolve(string name)
        {
            return resolvables.ContainsKey(name);
        }

        /// <summary>
        ///     Adds a Resolved Item
        /// </summary>
        /// <param name="name">Name of the Item</param>
        /// <param name="item">Item</param>
        public void AddResolved(string name, T item)
        {
            ResolvedCount++;
            resolvables.Add(name, item);
        }

        /// <summary>
        ///     Resolves a Symbol by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Resolve(string name)
        {
            return resolvables[name];
        }

        /// <summary>
        ///     Ends the Current Resolve Cycle
        /// </summary>
        public void EndCycle()
        {
            ResolvedCount = 0;
        }
    }
}