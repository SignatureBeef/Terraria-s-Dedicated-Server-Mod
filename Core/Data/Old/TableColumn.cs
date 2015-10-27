using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDSM.Core.Data.Old
{
    /// <summary>
    /// Table column.
    /// </summary>
    public struct TableColumn
    {
        public string Name { get; set; }

        public object DefaultValue { get; set; }

        public Type DataType { get; set; }

        public bool AutoIncrement { get; set; }

        public bool PrimaryKey { get; set; }

        public bool Nullable { get; set; }

        public int? MinScale { get; set; }

        public int? MaxScale { get; set; }

        public TableColumn(string name, Type dataType, bool autoIncrement, bool primaryKey, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = autoIncrement;
            this.PrimaryKey = primaryKey;
            this.Nullable = allowNulls;
            this.MinScale = null;
            this.MaxScale = null;
        }

        public TableColumn(string name, Type dataType, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = false;
            this.PrimaryKey = false;
            this.Nullable = allowNulls;
            this.MinScale = null;
            this.MaxScale = null;
        }

        public TableColumn(string name, Type dataType, int scale, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = false;
            this.PrimaryKey = false;
            this.Nullable = allowNulls;
            this.MinScale = scale;
            this.MaxScale = null;
        }
    }

}
