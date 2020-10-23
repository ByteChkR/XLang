using System.Collections.Generic;
using System.Linq;

namespace XLang.Parser.Expressions
{
    public class XLangExpressionOperatorCollection
    {

        private struct PrecedenceBucket
        {

            public readonly List<XLangExpressionOperator> bucket;

            public PrecedenceBucket(List<XLangExpressionOperator> operators)
            {
                bucket = operators;
            }
        }
        private Dictionary<int, PrecedenceBucket> buckets = new Dictionary<int, PrecedenceBucket>();

        public int Highest => buckets.Keys.Max();

        public int Lowest => buckets.Keys.Min();

        public XLangExpressionOperatorCollection(XLangExpressionOperator[] operators)
        {
            foreach (XLangExpressionOperator xLangExpressionOperator in operators)
            {
                if (buckets.ContainsKey(xLangExpressionOperator.PrecedenceLevel))
                    buckets[xLangExpressionOperator.PrecedenceLevel].bucket.Add(xLangExpressionOperator);
                else
                    buckets[xLangExpressionOperator.PrecedenceLevel] =
                        new PrecedenceBucket(new List<XLangExpressionOperator> { xLangExpressionOperator });
            }
        }

        public bool HasLevel(int level) => buckets.ContainsKey(level);

        public List<XLangExpressionOperator> GetLevel(int level) => buckets[level].bucket;

    }
}