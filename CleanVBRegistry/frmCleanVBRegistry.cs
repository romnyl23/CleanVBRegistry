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



        private Task FindKeys() {
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
                //                
                vPathern = $"({OptSaw + vSep1 + OptLib + vSep2 + OptConatbRpt})";
                string[] vTypeLib = Environment.Is64BitOperatingSystem ? new string[] { "WOW6432Node\\TypeLib","WOW6432Node\\CLSID" } : new string[] { "TypeLib","CLSID" };
                RegistryKey vRegKeyMaster = Registry.ClassesRoot;
                foreach(string vNodePath in vTypeLib) {
                    RegistryKey vRegSubKey = vRegKeyMaster.OpenSubKey(vNodePath,true);
                    foreach(string SubKeyName in vRegSubKey.GetSubKeyNames()) {
                        try {
                            RegistryKey vSubKey2 = vRegSubKey.OpenSubKey(SubKeyName,true);
                            foreach(string SubKeyDeepName in vSubKey2.GetSubKeyNames()) {
                                try {
                                    RegistryKey vSubKeyNode = vSubKey2.OpenSubKey(SubKeyDeepName,true);
                                    object vRegKeyValue = vSubKeyNode.GetValue("");
                                    if(vRegKeyValue != null && vRegKeyValue.ToString() != string.Empty) {
                                        if(Regex.IsMatch(vRegKeyValue.ToString(),vPathern,RegexOptions.IgnoreCase)) {
                                            if(vNodePath.Contains("TypeLib")) {
                                                InsertRegistryTab(ref RegistryTab,vSubKeyNode.Name,vRegKeyValue.ToString());
                                            } else {
                                                InsertRegistryTab(ref SubRegistryTab,vSubKeyNode.Name,vRegKeyValue.ToString(),true);
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
            });
            return vTask;
        }

        private void button1_Click(object sender,EventArgs e) {
            ExecuteFindKeysProcess();
        }

        private async void ExecuteFindKeysProcess() {
            await FindKeys();
            button2.Enabled = RegistryTab.Rows.Count > 0;
            FormatDataGridView(RegistryTab);
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

        private async void ExecuteDeleProcess(DataTable vTable) {
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
                                vRegKeyMaster.DeleteSubKey(fRegName);
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
            RegistryTab = (DataTable)dgvRegKey.DataSource;
            ExecuteDeleProcess(RegistryTab);
            ExecuteDeleProcess(SubRegistryTab);
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

