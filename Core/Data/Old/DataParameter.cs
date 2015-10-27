using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSM.Core.Data.Old
{
    /// <summary>
    /// Data parameter
    /// </summary>
    public struct DataParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DataParameter(string name, object value)
            : this()
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
