using P5XLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace P5XToolGUI
{
    public partial class PersonaControl : UserControl
    {
        bool initModelTab = false;
        bool initResourceTab = false;
        static int preferredTab = 0;
        long personaSn;
        public PersonaControl(long _personaSn)
        {
            personaSn = _personaSn;
            InitializeComponent();
            tabControl1.SelectedIndex = preferredTab;
            OnTabChange();
        }

        private void InitModelTab()
        {
            if (initModelTab) return;
            var personaData = DB.GetDataDict("Persona", "ConfPersona", personaSn);
            modelTab.Controls.Clear();
            if (personaData != null)
            {
                var modelControl = new ModelControl((string)personaData["model"]!)
                {
                    Dock = DockStyle.Fill
                };
                modelTab.Controls.Add(modelControl);
            }
            initModelTab = true;
        }
        private void InitResourceTab()
        {
            if (initResourceTab) return;
            var personaData = DB.GetDataDict("Persona", "ConfPersona", personaSn);
            if (personaData != null)
            {
                resourceControl1.SuspendResources();
                foreach (var kv in personaData)
                {
                    if (kv.Value is string s)
                    {
                        resourceControl1.AddResource(kv.Key, s);
                    }
                }
                resourceControl1.ResumeResources();
            }
            initResourceTab = true;
        }
        private void OnTabChange()
        {
            if (tabControl1.SelectedIndex == 0)
            {
                InitModelTab();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                InitResourceTab();
            }
            preferredTab = tabControl1.SelectedIndex;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            OnTabChange();
        }
    }
}
