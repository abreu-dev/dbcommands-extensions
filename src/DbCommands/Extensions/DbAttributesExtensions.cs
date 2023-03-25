using DbCommands.DbAttributes;
using System.Data;
using System.Reflection;

namespace DbCommands.Extensions
{
    public static class DbAttributesExtensions
    {
        public static DbTableAttribute GetDbTableAttributeOrThrowException(this Type type)
        {
            DbTableAttribute? dbTableAttribute = type.GetCustomAttribute<DbTableAttribute>();

            if (dbTableAttribute == null)
                throw new Exception("Was not possible to retrieve DbTableAttribute.");

            return dbTableAttribute;
        }

        public static IEnumerable<DbColumnAttribute> GetDbColumnAttributesOrThrowException(this Type type)
        {
            List<DbColumnAttribute> dbColumnAttributeList = new();

            foreach (PropertyInfo property in type.GetProperties())
            {
                DbColumnAttribute? dbColumnAttribute = property.GetCustomAttribute<DbColumnAttribute>();

                if (dbColumnAttribute != null)
                {
                    dbColumnAttribute.SetPropertyInfo(property);
                    dbColumnAttributeList.Add(dbColumnAttribute);
                }
            }

            if (dbColumnAttributeList.Count == 0)
                throw new Exception("Was not possible to retrieve any DbColumnAttribute.");

            if (dbColumnAttributeList.DistinctBy(k => k.ColumnIndex).Count() != dbColumnAttributeList.Count)
                throw new Exception("Type have repeated DbColumnAttribute indexes.");

            return dbColumnAttributeList.OrderBy(x => x.ColumnIndex);
        }

        public static DbColumnAttribute GetDbColumnPrimaryKeyAttributeOrThrowException(this Type type)
        {
            List<DbColumnAttribute> dbColumnAttributeList = new();

            foreach (PropertyInfo property in type.GetProperties())
            {
                DbColumnAttribute? dbColumnAttribute = property.GetCustomAttribute<DbColumnAttribute>();

                if (dbColumnAttribute != null && dbColumnAttribute.IsPrimaryKey)
                {
                    dbColumnAttribute.SetPropertyInfo(property);
                    dbColumnAttributeList.Add(dbColumnAttribute);
                }
            }

            if (dbColumnAttributeList.Count == 0)
                throw new Exception("Was not possible to retrieve any DbColumnAttribute with PrimaryKey.");

            if (dbColumnAttributeList.Count > 1)
                throw new Exception("Type have multiple DbColumnAttribute with PrimaryKey.");

            return dbColumnAttributeList.Single();
        }
    }
}
