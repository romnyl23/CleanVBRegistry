using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CleanVBRegistry {

    enum Action { 
        FindKeys,
        DeleteKeys
    }

    public partial class frmCleanVBRegistry:Form {
        Action eAction = new Action();
        DataTable RegistryTab;
        DataTable SubRegistryTab;
        public frmCleanVBRegistry() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            eAction = Action.FindKeys;            
            checkAll.Enabled = false;
            chLibGalac.Checked = true;
            txtLibGalac.Enabled = true;
            txtContabRpt.KeyPress += new KeyPressEventHandler(txtInputKey_Press);
            txtLibGalac.KeyPress += new KeyPressEventHandler(txtInputKey_Press);
            txtSaw.KeyPress += new KeyPressEventHandler(txtInputKey_Press);
        }

        private DataTable TabConfig() {
            DataTable vReq = new DataTable();
            vReq.Columns.Add("KeyRegName",typeof(string));
            vReq.Columns.Add("KeyRegValue",typeof(string));
            vReq.Columns.Add("Delete Key",typeof(bool));
            return vReq;
        }

        private void InsertRegistryTab(ref DataTable TableIn,string vKeyRegName,string KeyRegValue,bool isSubRegistryKey = false) {
            DataRow vRow = TableIn.NewRow();
            vRow["KeyRegName"] = vKeyRegName;
            vRow["KeyRegValue"] = KeyRegValue;
            vRow["Delete Key"] = isSubRegistryKey;
            TableIn.Rows.Add(vRow);
        }

        private void FinKeys(string valNodePath,string valFindPathern,ref DataTable valKeysTable) {
            RegistryKey vRegKeyMaster = Registry.ClassesRoot;
            RegistryKey vRegSubKey = vRegKeyMaster.OpenSubKey(valNodePath,true);
            foreach(string SubKeyName in vRegSubKey.GetSubKeyNames()) {
                try {
                    RegistryKey vSubKey2 = vRegSubKey.OpenSubKey(SubKeyName,true);
                    foreach(string SubKeyDeepName in vSubKey2.GetSubKeyNames()) {
                        try {
                            RegistryKey vSubKeyNode = vSubKey2.OpenSubKey(SubKeyDeepName,true);
                            object vRegKeyValue = vSubKeyNode.GetValue("");
                            if(vRegKeyValue != null && vRegKeyValue.ToString() != string.Empty) {
                                if(Regex.IsMatch(vRegKeyValue.ToString(),valFindPathern,RegexOptions.IgnoreCase)) {
                                    if(valNodePath.Contains("TypeLib")) {
                                        InsertRegistryTab(ref valKeysTable,vSubKeyNode.Name,vRegKeyValue.ToString());
                                    } else {
                                        InsertRegistryTab(ref valKeysTable,vSubKeyNode.Name,vRegKeyValue.ToString(),true);
                                    }
                                    break;
                                }
                            }
                        } catch(Exception yEx) {
                            throw yEx;
                        }
                    }
                } catch(Exception xEx) {
                    if(xEx.Message.Contains("Acceso denegado al Registro solicitado.")) {
                        continue;
                    } else {
                        throw xEx;
                    }
                }
            }
            vRegSubKey.Close();
        }

        private void ExecuteFindKeys() {
            try {
                RegistryTab = TabConfig();
                SubRegistryTab = TabConfig();
                string vPathern;
                string OptSaw, OptLib, OptConatbRpt, SawVersion, ContabVersion, LibGalacVersion, vSep1, vSep2;
                SawVersion = (txtSaw.Text != string.Empty) ? "_" + txtSaw.Text : "";
                ContabVersion = (txtContabRpt.Text != string.Empty) ? "_" + txtContabRpt.Text : "";
                LibGalacVersion = (txtLibGalac.Text != string.Empty) ? "_" + txtLibGalac.Text : "";
                OptSaw = !chSaw.Checked ? "" : SawVersion == string.Empty ? "Saw" : $"SawEnum{SawVersion}|SawImportExport{SawVersion}|SawMaquinaFiscal{SawVersion}|SawNav{SawVersion}|SawRpt{SawVersion}|SawSp{SawVersion}|SawTbls{SawVersion}|SawVista{SawVersion}";
                OptLib = !chLibGalac.Checked ? "" : LibGalacVersion == string.Empty ? "LibGalac" : $"LibGalacCI{LibGalacVersion}|LibGalaSecCI{LibGalacVersion}|LibGalacXtmCI{LibGalacVersion}|LibGalacRdpCI{LibGalacVersion}";
                OptConatbRpt = !chContabRpt.Checked ? "" : "ContabRpt" + ContabVersion;
                //
                vSep1 = OptSaw != string.Empty && (OptLib != string.Empty || OptConatbRpt != string.Empty) ? "|" : "";
                vSep2 = (OptLib != string.Empty && OptConatbRpt != string.Empty) ? "|" : "";
                vPathern = $"({OptSaw + vSep1 + OptLib + vSep2 + OptConatbRpt})";
                //
                string vNodePath = Environment.Is64BitOperatingSystem ? "WOW6432Node\\TypeLib" : "TypeLib";
                FinKeys(vNodePath,vPathern,ref RegistryTab);
                //
                vNodePath = Environment.Is64BitOperatingSystem ? "WOW6432Node\\CLSID" : "CLSID";
                FinKeys(vNodePath,vPathern,ref SubRegistryTab);
            } catch(Exception vEx) {
                MessageBox.Show(vEx.Message);
            }
        }

        private void button1_Click(object sender,EventArgs e) {
            eAction = Action.FindKeys;
            dgvRegKey.DataSource = null;
            ExecuteFindKeysProcess();
        }

        private void SetProgress(bool reset) {
            if(reset) {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.Value = 0;
            } else {
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = 100;
            }
        }

        private void ExecuteFindKeysProcess() {
            SetProgress(true);
            RunTask();                        
        }

        private void FormatDataGridView(DataTable vTable) {
            dgvRegKey.DataSource = RegistryTab;
            dgvRegKey.Columns[0].Visible = false;
            dgvRegKey.Columns[1].ReadOnly = true;
            dgvRegKey.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void frmCleanVBRegistry_Load(object sender,EventArgs e) {
            
        }

        private void txtInputKey_Press(object sender,KeyPressEventArgs e) {
            if(!(char.IsLetterOrDigit(e.KeyChar) || (Keys)e.KeyChar == Keys.Delete || (Keys)e.KeyChar == Keys.Back || (Keys)e.KeyChar == Keys.Enter || (Keys)e.KeyChar == Keys.Tab)) {
                e.KeyChar = '\0';
            }
        }

        

        private void DeleteRegistryKeys(DataTable vTable) {
            RegistryKey vRegKeyMaster = Registry.ClassesRoot;
            try {
                foreach(DataRow vRow in vTable.Rows) {
                    string fRegName = vRow["KeyRegName"].ToString();
                    bool vDeleteReg = (bool)vRow["Delete Key"];
                    fRegName = fRegName.Substring(fRegName.IndexOf("\\") + 1);
                    if(vDeleteReg) {
                        RegistryKey vRegSubKey = vRegKeyMaster.OpenSubKey(fRegName,true);
                        if(vRegSubKey != null) {
                            vRegKeyMaster.DeleteSubKeyTree(fRegName);
                        }
                        vRegKeyMaster.Close();
                    }
                }
            } catch(Exception vEx) {
                throw vEx;
            }
        }

        private void button2_Click(object sender,EventArgs e) {
            eAction = Action.DeleteKeys;
            dgvRegKey.EndEdit();
            RunTask();            
        }

        private void chLibGalac_CheckedChanged(object sender,EventArgs e) {
            button1.Enabled = chContabRpt.Checked || chLibGalac.Checked || chSaw.Checked;
            txtLibGalac.Enabled = chLibGalac.Checked;
        }

        private void chContabRpt_CheckedChanged(object sender,EventArgs e) {
            button1.Enabled = chContabRpt.Checked || chLibGalac.Checked || chSaw.Checked;
            txtContabRpt.Enabled = chContabRpt.Checked;
        }

        private void chSaw_CheckedChanged(object sender,EventArgs e) {
            button1.Enabled = chContabRpt.Checked || chLibGalac.Checked || chSaw.Checked;
            txtSaw.Enabled = chSaw.Checked;
        }

        public void RunTask() {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task vTask = Task.Factory.StartNew(() => {
                switch(eAction) {
                case Action.FindKeys:
                    ExecuteFindKeys();
                    break;
                case Action.DeleteKeys:
                    DeleteRegistryKeys(RegistryTab);
                    DeleteRegistryKeys(SubRegistryTab);
                    break;
                }
            });
            vTask.ContinueWith((t) => {
                switch(eAction) {
                case Action.FindKeys:
                    button2.Enabled = RegistryTab.Rows.Count > 0;
                    checkAll.Enabled = button2.Enabled;
                    FormatDataGridView(RegistryTab);
                    SetProgress(false);
                    break;
                case Action.DeleteKeys:
                    dgvRegKey.DataSource = null;
                    button2.Enabled = false;
                    checkAll.Enabled = false;
                    checkAll.Checked = false;
                    break;
                }
                if(t.IsCompleted) {
                    MessageBox.Show(this.Owner,"Finalizado con exíto",Text,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                } else {
                    MessageBox.Show(this.Owner,"Proceso falló " + t.Exception.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            },cancellationTokenSource.Token,TaskContinuationOptions.OnlyOnRanToCompletion,TaskScheduler.FromCurrentSynchronizationContext());
            vTask.ContinueWith((t) => {
                if(t.Exception.InnerException != null) {
                    MessageBox.Show(this.Owner,"Proceso falló " + t.Exception.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            },cancellationTokenSource.Token,TaskContinuationOptions.OnlyOnCanceled,TaskScheduler.FromCurrentSynchronizationContext());
            vTask.ContinueWith((t) => {
                if(t.Exception.InnerException != null) {
                    MessageBox.Show(this.Owner,"Proceso falló " + t.Exception.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
            },cancellationTokenSource.Token,TaskContinuationOptions.OnlyOnFaulted,TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void checkAll_CheckedChanged(object sender,EventArgs e) {
            try {
                if(checkAll.Checked) {
                    foreach(DataGridViewRow dr in dgvRegKey.Rows) {
                        if(dr.Cells[1].Value!=null){
                            dr.Cells[2].Value = true;
                        }
                    }
                }
            } catch(Exception vEx) {
                MessageBox.Show(this.Owner,vEx.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }
    }
}

