using DbCommands.DbAttributes;
using System.Data;
using System.Text;

namespace DbCommands.Extensions
{
    public static partial class DbCommandsExtensions
    {
        private const int DEFAULT_DELETE_BATCH_SIZE = 5;
        private const string DEFAULT_DELETE_TEMPLATE = "DELETE FROM {0}.{1} WHERE {2} IN ({3})";

        public static void DeleteInBatches<T, TPrimaryKey>(this IDbConnection dbConnection, IEnumerable<TPrimaryKey> objectIdentifiers)
        {
            Type type = typeof(T);

            DbTableAttribute dbTableAttribute = type.GetDbTableAttributeOrThrowException();
            DbColumnAttribute dbColumnPrimaryKeyAttribute = type.GetDbColumnPrimaryKeyAttributeOrThrowException();

            using IDbTransaction dbTransaction = dbConnection.BeginTransaction();
            using IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.Transaction = dbTransaction;

            string tableSchema = dbTableAttribute.SchemaName;
            string tableName = dbTableAttribute.TableName;
            DbType primaryKeyType = dbColumnPrimaryKeyAttribute.ColumnType;
            string primaryKeyName = dbColumnPrimaryKeyAttribute.ColumnName;

            int batchSize = DEFAULT_DELETE_BATCH_SIZE;
            string deleteTemplate = DEFAULT_DELETE_TEMPLATE;

            StringBuilder valuesBuilder = new();
            int currentOperationSize = 0;
            int parameterIndex = 0;

            foreach (TPrimaryKey identifier in objectIdentifiers)
            {
                string parameterName = string.Format("@p{0}", parameterIndex++);

                IDbDataParameter commandParameter = dbCommand.CreateParameter();
                commandParameter.ParameterName = parameterName;
                commandParameter.DbType = primaryKeyType;
                commandParameter.Value = identifier;
                dbCommand.Parameters.Add(commandParameter);

                string valueRow = string.Format("{0}", parameterName);

                if (currentOperationSize > 0)
                    valuesBuilder.AppendLine(",");
                valuesBuilder.Append(valueRow);

                currentOperationSize++;

                if (currentOperationSize == batchSize)
                {
                    string deleteSql = string.Format(deleteTemplate, tableSchema, tableName, primaryKeyName, valuesBuilder);
                    dbCommand.CommandText = deleteSql;
                    dbCommand.ExecuteNonQuery();

                    dbCommand.Parameters.Clear();
                    valuesBuilder.Clear();
                    currentOperationSize = 0;
                    parameterIndex = 0;
                }
            }

            if (currentOperationSize > 0)
            {
                string deleteSql = string.Format(deleteTemplate, tableSchema, tableName, "Id", valuesBuilder);
                dbCommand.CommandText = deleteSql;
                dbCommand.ExecuteNonQuery();
            }

            dbTransaction.Commit();
            dbConnection.Close();
        }
    }
}
