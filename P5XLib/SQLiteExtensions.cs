using SQLite;
using SQLitePCL;
using System.Text.Json;

namespace P5XLib
{
    public static class SQLiteExtensions
    {
        static void BindParameter(sqlite3_stmt stmt, int index, object value)
        {
            switch (value)
            {
                case int intValue:
                    SQLite3.BindInt(stmt, index, intValue);
                    break;
                case long longValue:
                    SQLite3.BindInt64(stmt, index, longValue);
                    break;
                case float floatValue:
                    SQLite3.BindDouble(stmt, index, floatValue);
                    break;
                case double doubleValue:
                    SQLite3.BindDouble(stmt, index, doubleValue);
                    break;
                case string stringValue:
                    SQLite3.BindText(stmt, index, stringValue, -1, new IntPtr(-1));
                    break;
                case byte[] byteArrayValue:
                    SQLite3.BindBlob(stmt, index, byteArrayValue, ((byte[])value).Length, new IntPtr(-1));
                    break;
                case null:
                    SQLite3.BindNull(stmt, index);
                    break;
                default:
                    throw new ArgumentException($"Unsupported parameter type: {value.GetType()}");
            }
        }
        public static List<Dictionary<string, object?>> QueryAsDict(this SQLiteConnection conn, string sql, params object[] args)
        {
            var statement = SQLite3.Prepare2(conn.Handle, sql);
            int num = 1;
            foreach (var arg in args)
            {
                BindParameter(statement, num, arg);
            }
            var colCount = SQLite3.ColumnCount(statement);
            var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            List<Dictionary<string, object?>> result = [];
            while (SQLite3.Step(statement) == SQLite3.Result.Row)
            {
                Dictionary<string, object?> row = [];
                for (int i = 0; i < colCount; i++)
                {
                    object? value = SQLite3.ColumnType(statement, i) switch
                    {
                        SQLite3.ColType.Integer => SQLite3.ColumnInt64(statement, i),
                        SQLite3.ColType.Float => SQLite3.ColumnDouble(statement, i),
                        SQLite3.ColType.Text => SQLite3.ColumnText(statement, i),
                        SQLite3.ColType.Blob => SQLite3.ColumnBlob(statement, i),
                        _ => null,
                    };
                    row.Add(SQLite3.ColumnName(statement, i), value);
                }
                result.Add(row);
            }
            SQLite3.Finalize(statement);
            return result;
        }
    }
}
