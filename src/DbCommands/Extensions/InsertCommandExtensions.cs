using DbCommands.DbAttributes;
using System.Data;
using System.Text;

namespace DbCommands.Extensions
{
    public static partial class DbCommandsExtensions
    {
        private const int DEFAULT_INSERT_BATCH_SIZE = 5;
        private const string DEFAULT_INSERT_TEMPLATE = "INSERT INTO {0}.{1} ({2}) VALUES {3}";

        public static void InsertInBatches<T>(this IDbConnection dbConnection, IEnumerable<T> objectList)
        {
            Type type = typeof(T);

            DbTableAttribute dbTableAttribute = type.GetDbTableAttributeOrThrowException();
            IEnumerable<DbColumnAttribute> dbColumnAttributeList = type.GetDbColumnAttributesOrThrowException();

            using IDbTransaction dbTransaction = dbConnection.BeginTransaction();
            using IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.Transaction = dbTransaction;

            string tableSchema = dbTableAttribute.SchemaName;
            string tableName = dbTableAttribute.TableName;
            string tableColumnNameList = string.Join(", ", dbColumnAttributeList.Select(s => s.ColumnName));

            int batchSize = DEFAULT_INSERT_BATCH_SIZE;
            string insertTemplate = DEFAULT_INSERT_TEMPLATE;

            StringBuilder valuesBuilder = new();
            int currentOperationSize = 0;
            int parameterIndex = 0;

            foreach (T @object in objectList)
            {
                if (@object == null) return;

                List<string> parameterNameList = new();

                foreach (DbColumnAttribute dbColumnAttribute in dbColumnAttributeList)
                {
                    string parameterName = string.Format("@p{0}", parameterIndex++);
                    parameterNameList.Add(parameterName);

                    IDbDataParameter commandParameter = dbCommand.CreateParameter();
                    commandParameter.ParameterName = parameterName;
                    commandParameter.DbType = dbColumnAttribute.ColumnType;
                    commandParameter.Value = dbColumnAttribute.GetValue(@object);
                    dbCommand.Parameters.Add(commandParameter);
                }

                string valueRow = string.Format("({0})", string.Join(", ", parameterNameList));

                if (currentOperationSize > 0)
                    valuesBuilder.AppendLine(",");
                valuesBuilder.Append(valueRow);

                currentOperationSize++;

                if (currentOperationSize == batchSize)
                {
                    string insertSql = string.Format(insertTemplate, tableSchema, tableName, tableColumnNameList, valuesBuilder);
                    dbCommand.CommandText = insertSql;
                    dbCommand.ExecuteNonQuery();

                    dbCommand.Parameters.Clear();
                    valuesBuilder.Clear();
                    currentOperationSize = 0;
                    parameterIndex = 0;
                }
            }

            if (currentOperationSize > 0)
            {
                string insertSql = string.Format(insertTemplate, tableSchema, tableName, tableColumnNameList, valuesBuilder);
                dbCommand.CommandText = insertSql;
                dbCommand.ExecuteNonQuery();
            }

            dbTransaction.Commit();
            dbConnection.Close();
        }
    }
}
