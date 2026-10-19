using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using P5XTool;

namespace P5XToolGUI
{
    public partial class OptionForm : Form
    {
        public Config.User userPrefs;
        public OptionForm()
        {
            InitializeComponent();
            userPrefs = Config.GetUserConfig().Clone();
            autoLoadCheck.Checked = userPrefs.AutoLoad;
            autoPathText.Text = userPrefs.AutoPath;
            AutoCompleteStringCollection suggestions = [.. Config.GetRecents()];
            autoPathText.AutoCompleteCustomSource = suggestions;
        }

        private void autoLoadCheck_CheckedChanged(object sender, EventArgs e)
        {
            userPrefs.AutoLoad = autoLoadCheck.Checked;
        }

        private void autoPathText_TextChanged(object sender, EventArgs e)
        {
            userPrefs.AutoPath = autoPathText.Text;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Config.SetUserConfig(userPrefs);
        }

        private void dirSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                autoPathText.Text = dialog.SelectedPath;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
