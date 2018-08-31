using HL7SenderService.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HL7SenderService.View
{
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        public DataTable dtConfig = CreateDataTable();
        private static DataTable CreateDataTable()
        {
            DataTable objTable = new DataTable();
            objTable.Columns.Add("ColumnTitle", typeof(String));
            objTable.Columns.Add("ColumnValue", typeof(String));

            return objTable;
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            this.Text = DoService.pubService + " Config " + " Ver." + DoService.pubVersion;

            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                DataRow row = dtConfig.NewRow();

                row["ColumnTitle"] = currentProperty.Name;
                row["ColumnValue"] = Properties.Settings.Default[currentProperty.Name];

                dtConfig.Rows.Add(row);
            }

            dtConfig = SetTableFilter(dtConfig, "ColumnTitle", "");
            dataGridView1.DataSource = dtConfig;
        }

        public static DataTable SetTableFilter(DataTable _objTable, string _Sort, string _Filter)
        {
            DataTable objTable = new DataTable();

            try
            {
                DataView dvFilter = _objTable.DefaultView;

                if (_objTable.Rows.Count > 0)
                {
                    dvFilter.Sort = _Sort;
                    dvFilter.RowFilter = _Filter;
                }

                if (dvFilter.Count > 0)
                {
                    objTable = dvFilter.ToTable();
                }
            }
            catch
            { }

            return objTable;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                dtConfig.AcceptChanges();

                string path = Application.ExecutablePath + ".config";
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                if (File.Exists(path))
                {
                    doc.Load(path);

                    foreach (DataRow row in dtConfig.Rows)
                    {
                        string configString = @"configuration/applicationSettings/" + DoService.pubService
                            + ".Properties.Settings/setting[@name='" + row["ColumnTitle"].ToString() + "']/value";

                        System.Xml.XmlNode configNode = doc.SelectSingleNode(configString);
                        if (configNode != null)
                        {
                            configNode.InnerText = row["ColumnValue"].ToString();
                        }
                    }
                    doc.Save(path);
                    Properties.Settings.Default.Reload();
                }

                DoService.WriteEventLog(DoService.pubService + " Save Config Success!");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                DoService.WriteEventLog(DoService.pubService + " Save Config Failed!" + ex.Message);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
