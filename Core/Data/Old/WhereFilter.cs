using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSM.Core.Data.Old
{
    /// <summary>
    /// Where filter.
    /// </summary>
    public struct WhereFilter
    {
        public string Column { get; set; }

        public object Value { get; set; }

        public WhereExpression Expression { get; set; }

        public WhereFilter(string column, object value, WhereExpression expression = WhereExpression.EqualTo)
            : this()
        {
            this.Expression = expression;
            this.Column = column;
            this.Value = value;
        }
    }

    /// <summary>
    /// Where expression.
    /// </summary>
    public enum WhereExpression : byte
    {
        EqualTo,
        Like,
        NotEqualTo
    }
}
