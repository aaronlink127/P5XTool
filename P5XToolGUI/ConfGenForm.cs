using P5XLib;
using System.Data;
using SQLite;

namespace P5XToolGUI
{
    public partial class ConfGenForm : Form
    {
        public static Dictionary<string, string> Conf2DbName = [];
        public ConfGenForm()
        {
            InitializeComponent();
            confSel.Items.Clear();
            var tbls = DB.GetAllTables().OrderBy((t) => t.Item1);
            foreach (var tbl in tbls)
            {
                Conf2DbName[tbl.Item1] = tbl.Item2;
                confSel.Items.Add(tbl.Item1);
            }
        }

        class ColInfo
        {
            [PrimaryKey]
            public int Cid { get; set; }
            [Indexed]
            [Column("name")]
            public string Name { get; set; } = "";
            [Column("type")]
            public string Type { get; set; } = "";
            [Column("notnull")]
            public int NotNull { get; set; }
            [Column("dflt_value")]
            public string? DefaultValue { get; set; } = "";
            [Column("pk")]
            public int Pk { get; set; }
        }

        private void updateGen()
        {
            var dbMap = DB.GetDBMap();
            string? selectedConf = confSel.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedConf)) return;
            string guessedExcelName = selectedConf[4..];
            var dbName = Conf2DbName[selectedConf];
            for (int i = guessedExcelName.Length; i > 0; i--)
            {
                string potentialName = guessedExcelName[..i];
                if (dbMap.TryGetValue(potentialName, out var potentialDbName) && potentialDbName == dbName)
                {
                    guessedExcelName = potentialName;
                    break;
                }
            }
            string fullStr = $"public class {guessedExcelName} {{\r\n    public string _ExcelName = \"{guessedExcelName}\";\r\n    public string _FileName = \"{selectedConf}\";\r\n";
            var conn = DB.GetDBConnection(dbName);
            var tblInfo = conn.Query<ColInfo>($"PRAGMA table_info({selectedConf})");
            foreach (var col in tblInfo)
            {
                string colType = "string";
                switch (col.Type.ToLower())
                {
                    case "integer":
                        colType = "int";
                        break;
                    case "float":
                        colType = "float";
                        break;
                    case "bool":
                        colType = "bool";
                        break;
                    case "text":
                        colType = "string";
                        break;
                    default:
                        colType = $"string /*{col.Type}*/";
                        break;
                }
                string colName = col.Name;
                if (titleCaseCb.Checked)
                {
                    colName = char.ToUpper(colName[0]) + colName[1..];
                }
                List<string> additionalAttrs = [];
                if (col.Pk == 1)
                {
                    additionalAttrs.Add("PrimaryKey");
                }
                if (explicitColumnCb.Checked)
                {
                    additionalAttrs.Add($"Column(\"{col.Name}\")");
                }
                if (additionalAttrs.Count > 0)
                {
                    fullStr += $"    [{string.Join(", ", additionalAttrs)}]\r\n";
                }
                fullStr += $"    const {colType} {colName} {{ get; set; }}\r\n";
            }
            fullStr += "}\r\n";
            textBox1.Text = fullStr;
        }

        private void explicitColumnCb_CheckedChanged(object sender, EventArgs e)
        {
            updateGen();
        }

        private void confSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateGen();
        }

        private void titleCaseCb_CheckedChanged(object sender, EventArgs e)
        {
            updateGen();
        }
    }
}
