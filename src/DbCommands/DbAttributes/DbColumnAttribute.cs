using System.Data;
using System.Reflection;

namespace DbCommands.DbAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DbColumnAttribute : Attribute
    {
        private readonly string _columnName;
        private readonly DbType _columnType;
        private readonly int _columnIndex;

        private PropertyInfo? _propertyInfo;

        public DbColumnAttribute(string columnName, DbType columnType, int columnIndex)
        {
            _columnName = columnName;
            _columnType = columnType;
            _columnIndex = columnIndex;
        }

        public string ColumnName => _columnName;
        public DbType ColumnType => _columnType;
        public int ColumnIndex => _columnIndex;
        public bool IsPrimaryKey { get; set; }

        public void SetPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo != null)
                _propertyInfo = propertyInfo;
        }

        public object GetValue(object @object)
        {
            if (_propertyInfo == null) return DBNull.Value;

            var value = _propertyInfo.GetValue(@object);
            if (value == null) return DBNull.Value;

            return value;
        }
    }
}
