using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace P5XLib
{
    using TablePair = (string table, string dbName);
    public static class DB
    {
        static Dictionary<string, string> configDBMap = [];
        static List<string> translationConfigs = [];
        static string basicDBName = "ConfDataDefault";
        static Dictionary<string, SQLiteConnection> DBConnections = [];
        static Dictionary<string, string> dbKeyMap = [];
        static bool isInit = false;
        static public bool IsInit => isInit;
        public static string GetDBForConfig(string excelName)
        {
            if (configDBMap.TryGetValue(excelName, out string? dbName))
            {
                return dbName;
            }
            return basicDBName;
        }
        public static SQLiteConnection GetDBConnection(string dbName)
        {
            if (DBConnections.TryGetValue(dbName, out SQLiteConnection? conn))
            {
                return conn;
            }

            string dbPath = VFileSystem.GetRealPath($"Config/Windows/{dbName}.bytes") ?? throw new Exception($"Failed to find DB {dbName}");
            SQLiteConnectionString options;
            if (dbKeyMap.TryGetValue(dbName, out string? key))
            {
                options = new SQLiteConnectionString(dbPath, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex, true, key);
            }
            else
            {
                options = new SQLiteConnectionString(dbPath, SQLiteOpenFlags.ReadOnly | SQLiteOpenFlags.FullMutex, true);
            }
            conn = new SQLiteConnection(options);
            DBConnections.Add(dbName, conn);
            return conn;
        }
        class NameWrap
        {
            public string Name { get; set; } = "";
        }
        public static List<string> GetTablesInDB(string dbName)
        {
            SQLiteConnection conn = GetDBConnection(dbName);
            List<string>  tableNames = [];
            var query = conn.Query<NameWrap>("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name");
            
            foreach (var tableInfo in query)
            {
                tableNames.Add(tableInfo.Name);
            }
            return tableNames;
        }
        public static string GetBasicDBName()
        {
            return basicDBName;
        }
        public static List<string> GetTLList()
        {
            return translationConfigs;
        }
        public static Dictionary<string, string> GetDBMap()
        {
            return configDBMap;
        }
        public static List<string> GetAllDBNames()
        {
            return [.. configDBMap.Values.Distinct(), basicDBName];
        }

        public static List<TablePair> GetAllTables()
        {
            HashSet<string> allTables = [];
            List<TablePair> tableNames = [];
            foreach (var dbName in configDBMap.Values.Distinct())
            {
                var tables = GetTablesInDB(dbName);
                foreach (var table in tables)
                {
                    if (allTables.Contains(table)) continue;
                    allTables.Add(table);
                    tableNames.Add(new(table, dbName));
                }
            }
            var basicTables = GetTablesInDB(basicDBName);
            foreach (var table in basicTables)
            {
                if (allTables.Contains(table)) continue;
                allTables.Add(table);
                tableNames.Add(new(table, basicDBName));
            }
            return tableNames;
        }
        public static List<T> Query<T>(string sql, params object[] args) where T : new()
        {
            string? excelName = typeof(T).GetField("_ExcelName")?.GetValue(null) as string ?? throw new ArgumentException("type missing _ExcelName attribute");
            string? fileName = typeof(T).GetField("_FileName")?.GetValue(null) as string ?? throw new ArgumentException("type missing _FileName attribute");
            SQLiteConnection conn = GetDBConnection(GetDBForConfig(excelName));
            return conn.Query<T>(sql, args);
        }
        public static List<Dictionary<string, object?>> QueryDict(string excelName, string sql, params object[] args)
        {
            SQLiteConnection conn = GetDBConnection(GetDBForConfig(excelName));
            return conn.QueryAsDict(sql, args);
        }
        public static List<T> GetTable<T>() where T : new()
        {
            string? excelName = typeof(T).GetField("_ExcelName")?.GetValue(null) as string ?? throw new ArgumentException("type missing _ExcelName attribute");
            string? fileName = typeof(T).GetField("_FileName")?.GetValue(null) as string ?? throw new ArgumentException("type missing _FileName attribute");
            SQLiteConnection conn = GetDBConnection(GetDBForConfig(excelName));
            return conn.Query<T>($"SELECT * FROM {fileName}");
        }
        public static List<Dictionary<string, object?>> GetTableDict(string excelName, string fileName)
        {
            SQLiteConnection conn = GetDBConnection(GetDBForConfig(excelName));
            return conn.QueryAsDict($"SELECT * FROM {fileName}");
        }
        public static T? GetData<T>(object sn) where T : new()
        {
            string? excelName = typeof(T).GetField("_ExcelName")?.GetValue(null) as string ?? throw new ArgumentException("type missing _ExcelName attribute");
            string? fileName = typeof(T).GetField("_FileName")?.GetValue(null) as string ?? throw new ArgumentException("type missing _FileName attribute");
            SQLiteConnection conn = GetDBConnection(GetDBForConfig(excelName));
            return conn.Query<T>($"SELECT * FROM {fileName} WHERE sn = ?", sn).FirstOrDefault();
        }
        public static Dictionary<string, object?>? GetDataDict(string excelName, string fileName, object sn)
        {
            SQLiteConnection conn = GetDBConnection(GetDBForConfig(excelName));
            return conn.QueryAsDict($"SELECT * FROM {fileName} WHERE sn = ?", sn).FirstOrDefault();
        }
        public static bool TryInit()
        {
            if (isInit) Reset();
            DBConnections = [];
            XmlDocument doc = new();
            using Stream? dbStream = VFileSystem.Open("Config/Windows/ConfigDBMap.bytes");
            if (dbStream == null) return false;
            doc.Load(dbStream);
            XmlElement? root = doc.DocumentElement;
            if (root == null) return false;
            basicDBName = root.GetAttribute("BasicDBName");
            foreach (XmlNode node in root.SelectSingleNode("Map")!.ChildNodes)
            {
                configDBMap.Add(node.Attributes!["ExcelName"]!.Value, node.Attributes["DBName"]!.Value);
            }
            var transFiles = root.SelectSingleNode("TranslationConfigFiles");
            if (transFiles != null)
            {
                foreach (XmlNode node in transFiles.ChildNodes)
                {
                    translationConfigs.Add(node.InnerText);
                }
            }
            if (!GameMgr.IsCbt)
            {
                try
                {
                    dbKeyMap = Lua.GetKeys();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to get DB keys");
                    Console.WriteLine(e);
                    return false;
                }
            }
            isInit = true;
            return true;
        }
        public static void CloseAllConnections()
        {
            foreach (var conn in DBConnections.Values)
            {
                conn.Close();
            }
            DBConnections = [];
        }
        public static void Reset()
        {
            CloseAllConnections();
            configDBMap.Clear();
            dbKeyMap.Clear();
            isInit = false;
        }
    }
}
