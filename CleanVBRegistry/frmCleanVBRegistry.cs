using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CleanVBRegistry {

    public partial class frmCleanVBRegistry:Form {
        DataTable RegistryTab;

        public frmCleanVBRegistry() {
            InitializeComponent();
        }

        private DataTable TabConfig() {
            DataTable vReq = new DataTable();
            vReq.Columns.Add("KeyRegName",typeof(string));
            vReq.Columns.Add("KeyRegValue",typeof(string));
            vReq.Columns.Add("Delete Key",typeof(bool));
            return vReq;
        }

        private void InsertRegistryTab(ref DataTable TableIn,string vKeyRegName,string KeyRegValue) {
            DataRow vRow = TableIn.NewRow();
            vRow["KeyRegName"] = vKeyRegName;
            vRow["KeyRegValue"] = KeyRegValue;
            vRow["Delete Key"] = false;
            TableIn.Rows.Add(vRow);
        }

        private void FindKeys() {
            RegistryTab = TabConfig();
            string vPathern = @"(SawEnum|SawImportExport|SawMaquinaFiscal|SawNav|SawRpt|SawSP|SawTbls|SawVista|LibGalac|ContabRpt)";
            string vTypeLib = Environment.Is64BitOperatingSystem ? "WOW6432Node\\TypeLib" : "TypeLib";
            RegistryKey vRegKeyMaster = Registry.ClassesRoot;
            RegistryKey vRegSubKey = vRegKeyMaster.OpenSubKey(vTypeLib,true);
            foreach(string SubKeyName in vRegSubKey.GetSubKeyNames()) {
                try {
                    RegistryKey vSubKey2 = vRegSubKey.OpenSubKey(SubKeyName,true);
                    foreach(string SubKeyDeepName in vSubKey2.GetSubKeyNames()) {
                        try {
                            var vSubKr = vSubKey2.OpenSubKey(SubKeyDeepName,true);
                            var vF = vSubKr.GetValue("");
                            if(vF != null) {
                                if(Regex.IsMatch(vF.ToString(),vPathern,RegexOptions.IgnoreCase)) {
                                    //vReg.DeleteSubKeyTree(xRegName);
                                    //here
                                    RegistryKey vSubKeyNode = vSubKey2.OpenSubKey(SubKeyDeepName,true);
                                    object vRegKeyValue = vSubKeyNode.GetValue("");
                                    if(vRegKeyValue != null && vRegKeyValue.ToString() != string.Empty) {
                                        if(Regex.IsMatch(vRegKeyValue.ToString(),vPathern,RegexOptions.IgnoreCase)) {
                                            InsertRegistryTab(ref RegistryTab,vSubKeyNode.Name,vRegKeyValue.ToString());
                                            break;
                                        }
                                    }
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
            FormatDataGridView(RegistryTab);
        }

        private void button1_Click(object sender,EventArgs e) {
            FindKeys();
        }


        private void FormatDataGridView(DataTable vTable) {
            dgvRegKey.DataSource = RegistryTab;
            dgvRegKey.Columns[0].Visible = false;
            dgvRegKey.Columns[1].ReadOnly = true;
            dgvRegKey.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void frmCleanVBRegistry_Load(object sender,EventArgs e) {

        }

        private void DeleteRegistry(DataTable vTable) {

            foreach(DataRow vRow in vTable.Rows) {
                vRow["KeyRegName"] = "";            
            }
        
        }

        private void button2_Click(object sender,EventArgs e) {
            dgvRegKey.EndEdit();
            RegistryTab = (DataTable)dgvRegKey.DataSource;
            DeleteRegistry(RegistryTab);
        }
    }
}
