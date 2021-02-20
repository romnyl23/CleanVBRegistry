using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CleanVBRegistry {

    public partial class frmCleanVBRegistry:Form {
        DataTable RegistryTab;
        DataTable SubRegistryTab;        
        public frmCleanVBRegistry() {
            InitializeComponent();
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

        private void InsertRegistryTab(ref DataTable TableIn,string vKeyRegName,string KeyRegValue, bool isSubRegistryKey=false) {
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

        private Task ExecuteFindKeys() {
            RegistryTab = TabConfig();
            SubRegistryTab = TabConfig();
            string vPathern;
            string OptSaw, OptLib, OptConatbRpt, SawVersion, ContabVersion, LibGalacVersion, vSep1, vSep2;

            Task vTask = Task.Factory.StartNew(() => {
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
            });
            return vTask;
        }

        private void button1_Click(object sender,EventArgs e) {
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

        private async void ExecuteFindKeysProcess() {
            SetProgress(true);
            await ExecuteFindKeys();            
            button2.Enabled = RegistryTab.Rows.Count > 0;
            FormatDataGridView(RegistryTab);
            SetProgress(false);
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

        private async void ExecuteDeleteProcess(DataTable vTable) {
            await DeleteRegistry( vTable);
            dgvRegKey.DataSource = null;
        }

        private Task DeleteRegistry(DataTable vTable) {
            Task vTask = Task.Factory.StartNew(() => {
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
                } catch(Exception) {
                    throw;
                }

            });
            return vTask;
        }

        private void button2_Click(object sender,EventArgs e) {
            dgvRegKey.EndEdit();            
            ExecuteDeleteProcess(RegistryTab);
            ExecuteDeleteProcess(SubRegistryTab);
            button2.Enabled = false;
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
    }
}

