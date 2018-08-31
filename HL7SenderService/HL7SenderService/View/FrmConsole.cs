using HL7SenderService.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HL7SenderService.View
{
    public partial class FrmConsole : Form
    {
        public FrmConsole()
        {
            InitializeComponent();
        }

        private void FrmConsole_Load(object sender, EventArgs e)
        {

            this.Text = DoService.pubService + " View " + " Ver." + DoService.pubVersion;

            if (!EventLog.SourceExists(DoService.pubService + "Src"))
            {
                EventLog.CreateEventSource(DoService.pubService + "Src", DoService.pubService + "Event");
            }

            this.btnStartSvc_Click(sender, e);
        }

        private void btnStartSvc_Click(object sender, EventArgs e)
        {
            DoService.Start();

            btnStartSvc.Enabled = false;
            btnStopSvc.Enabled = true;
        }

        private void btnStopSvc_Click(object sender, EventArgs e)
        {
            DoService.Stop();

            btnStartSvc.Enabled = true;
            btnStopSvc.Enabled = false;
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            FrmConfig objConfig = new FrmConfig();

            if (objConfig.ShowDialog(this) == DialogResult.OK)
            {
                btnStartSvc_Click(sender, e);
                btnStopSvc_Click(sender, e);
            }

            objConfig.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            EventLog objEventLog = new EventLog();
            objEventLog.Source = DoService.pubService + "Src";

            List<EventLogEntry> lstEventLog = objEventLog.Entries.OfType<EventLogEntry>()
                .Where(
                    log =>
                    log.Source == DoService.pubService + "Src")
                .ToList();

            string logMessage = "";
            int iLoopMax = 10;
            if (lstEventLog.Count() < iLoopMax) iLoopMax = lstEventLog.Count();
            for (int iLoop = 1; iLoop <= iLoopMax; iLoop++)
            {
                logMessage += "\r\n" + lstEventLog[lstEventLog.Count() - iLoop].Message;
            }

            txtMessage.Text = logMessage;
        }
    }
}
