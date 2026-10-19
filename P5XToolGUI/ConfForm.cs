using P5XLib;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace P5XToolGUI
{
    public partial class ConfForm : Form
    {
        public string SelectedConf => confSel.SelectedItem as string ?? "";
        enum Format
        {
            CSV,
            JSON
        }
        public static Dictionary<string, string> Conf2DbName = [];
        public ConfForm()
        {
            InitializeComponent();
            Conf2DbName = [];
            confSel.Items.Clear();
            var tbls = DB.GetAllTables().OrderBy((t) => t.table);
            //fmtSel.SelectedIndex = 0;
            foreach (var tbl in tbls)
            {
                Conf2DbName[tbl.Item1] = tbl.Item2;
                confSel.Items.Add(tbl.Item1);
            }
        }

        private static string EscapeCsv(string s)
        {
            if (s.Contains("\"") || s.Contains(",") || s.Contains("\n"))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }
        static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            IndentSize = 4,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        private static void SaveConfig(string fileName, string filePath, Format fmt)
        {
            var conn = DB.GetDBConnection(Conf2DbName[fileName]);
            var data = conn.QueryAsDict($"SELECT * FROM {fileName}");
            if (fmt == Format.JSON)
            {
                using var writer = new StreamWriter(filePath, false);
                string jsonString = JsonSerializer.Serialize(data, jsonOptions);
                writer.Write(jsonString);
            }
            else if (fmt == Format.CSV)
            {
                using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
                var columns = conn.GetTableInfo(fileName);
                var columnNames = columns.Select(c => c.Name).ToList();
                writer.WriteLine(string.Join(",", columnNames));
                foreach (var row in data)
                {
                    List<string> rowValues = [];
                    foreach (var colName in columnNames)
                    {
                        if (row.TryGetValue(colName, out var value) && value != null)
                        {
                            rowValues.Add(EscapeCsv(value.ToString() ?? ""));
                        }
                        else
                        {
                            rowValues.Add("");
                        }
                    }
                    writer.WriteLine(string.Join(",", rowValues));
                }
            }
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedConf))
            {
                return;
            }
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Save as CSV", null, (s, ea) =>
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.FileName = SelectedConf + ".csv";
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                var result = saveFileDialog.ShowDialog(this);
                if (result != DialogResult.OK) return;
                var filePath = saveFileDialog.FileName;
                SaveConfig(SelectedConf, filePath, Format.CSV);
            });
            contextMenu.Items.Add("Save as JSON", null, (s, ea) =>
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.FileName = SelectedConf + ".json";
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                var result = saveFileDialog.ShowDialog(this);
                if (result != DialogResult.OK) return;
                var filePath = saveFileDialog.FileName;
                SaveConfig(SelectedConf, filePath, Format.JSON);
            });
            contextMenu.Show(okButton, new Point(0, -contextMenu.Height));
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SaveConfigs(string path, Format fmt)
        {
            Enabled = false;
            GUtils.LongTask(this, "Saving Configs", (report) =>
            {
                int total = Conf2DbName.Keys.Count;
                int count = 0;
                foreach (var conf in Conf2DbName.Keys)
                {
                    report(count * 100 / total, $"{conf} ({count}/{total})");
                    var ext = fmt == Format.CSV ? ".csv" : ".json";
                    var filePath = Path.Combine(path, conf + ext);
                    SaveConfig(conf, filePath, fmt);
                    count++;
                }
                return null;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Save as CSV", null, (s, ea) =>
            {
                var result = folderBrowserDialog1.ShowDialog(this);
                if (result != DialogResult.OK) return;
                var csvPath = folderBrowserDialog1.SelectedPath;
                SaveConfigs(csvPath, Format.CSV);
            });
            contextMenu.Items.Add("Save as JSON", null, (s, ea) =>
            {
                var result = folderBrowserDialog1.ShowDialog(this);
                if (result != DialogResult.OK) return;
                var csvPath = folderBrowserDialog1.SelectedPath;
                SaveConfigs(csvPath, Format.JSON);
            });
            contextMenu.Show(button1, new Point(0, -contextMenu.Height));
        }

        private void confSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            string? selectedConf = confSel.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedConf)) return;
            var conn = DB.GetDBConnection(Conf2DbName[selectedConf]);
            var columns = conn.GetTableInfo(selectedConf);
            foreach (var col in columns)
            {
                dataGridView1.Columns.Add(col.Name, col.Name);
            }
            var data = conn.QueryAsDict($"SELECT * FROM {selectedConf}");
            foreach (var row in data)
            {
                List<object> rowValues = [];
                foreach (var col in columns)
                {
                    if (row.TryGetValue(col.Name, out var value) && value != null)
                    {
                        var val = value.ToString() ?? "";
                        rowValues.Add(val);
                    }
                    else
                    {
                        rowValues.Add("");
                    }
                }
                dataGridView1.Rows.Add(rowValues.ToArray());
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Copy", null, (s, ea) =>
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    {
                        var cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if (cellValue != null)
                        {
                            Clipboard.SetText(cellValue.ToString() ?? "");
                        }
                    }
                });
                var cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                if (cellValue != null && cellValue.EndsWith(".png") == true)
                {
                    contextMenu.Items.Add("Open Sprite", null, (s, ea) =>
                    {
                        if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                        {
                            if (cellValue != null)
                            {
                                var spriteName = cellValue.ToString() ?? "";
                                var spriteForm = new SpriteForm();
                                spriteForm.SetSprite(spriteName);
                                spriteForm.Icon = Icon;
                                spriteForm.Show(this);
                            }
                        }
                    });
                }
                var rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                contextMenu.Show(dataGridView1, new(e.Location.X + rect.Location.X, e.Location.Y + rect.Location.Y));
            }
        }
    }
}
