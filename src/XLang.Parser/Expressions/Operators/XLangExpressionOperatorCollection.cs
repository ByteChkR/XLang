using System.Collections.Generic;
using System.Linq;

namespace XLang.Parser.Expressions
{
    public class XLangExpressionOperatorCollection
    {
        private readonly Dictionary<int, PrecedenceBucket> buckets = new Dictionary<int, PrecedenceBucket>();

        public XLangExpressionOperatorCollection(XLangExpressionOperator[] operators)
        {
            foreach (XLangExpressionOperator xLangExpressionOperator in operators)
            {
                if (buckets.ContainsKey(xLangExpressionOperator.PrecedenceLevel))
                {
                    buckets[xLangExpressionOperator.PrecedenceLevel].bucket.Add(xLangExpressionOperator);
                }
                else
                {
                    buckets[xLangExpressionOperator.PrecedenceLevel] =
                        new PrecedenceBucket(new List<XLangExpressionOperator> {xLangExpressionOperator});
                }
            }
        }

        public int Highest => buckets.Keys.Max();

        public int Lowest => buckets.Keys.Min();

        public bool HasLevel(int level)
        {
            return buckets.ContainsKey(level);
        }

        public List<XLangExpressionOperator> GetLevel(int level)
        {
            return buckets[level].bucket;
        }

        private struct PrecedenceBucket
        {
            public readonly List<XLangExpressionOperator> bucket;

            public PrecedenceBucket(List<XLangExpressionOperator> operators)
            {
                bucket = operators;
            }
        }
    }
}