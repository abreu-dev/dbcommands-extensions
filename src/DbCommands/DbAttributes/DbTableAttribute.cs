namespace DbCommands.DbAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DbTableAttribute : Attribute
    {
        private readonly string _schemaName;
        private readonly string _tableName;

        public DbTableAttribute(string schemaName, string tableName)
        {
            _schemaName = schemaName;
            _tableName = tableName;
        }

        public string SchemaName => _schemaName;
        public string TableName => _tableName;
    }
}
