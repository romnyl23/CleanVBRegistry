using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CleanVBRegistry {
    public partial class frmCleanVBRegistry:Form {
        public frmCleanVBRegistry() {
            InitializeComponent();
        }        
        private void button1_Click(object sender,EventArgs e) {
            string vPathern = @"(SawEnum|SawImportExport|SawMaquinaFiscal|SawNav|SawRpt|SawSP|SawTbls|SawVista|LibGalac|ContabRpt)";
            RegistryKey vRegKey = Registry.ClassesRoot;                 
            string vTypeLib = Environment.Is64BitOperatingSystem ? "WOW6432Node\\TypeLib" : "TypeLib";
            var vReg = vRegKey.OpenSubKey(vTypeLib,true);            
            foreach(string xRegName in vReg.GetSubKeyNames()) {
                try {
                    var vSubK = vReg.OpenSubKey(xRegName,true);
                    foreach(string zRegName in vSubK.GetSubKeyNames()) {
                        try {
                            var vSubKr = vSubK.OpenSubKey(zRegName,true);
                            var vF = vSubKr.GetValue("");
                            if(vF != null) {                                
                                if(Regex.IsMatch(vF.ToString(),vPathern,RegexOptions.IgnoreCase)) {                                    
                                    vReg.DeleteSubKeyTree(xRegName);
                                    //here
                                    break;
                                }
                            }
                        } catch(Exception yEx) {
                            throw yEx;
                        }
                    }                    
                } catch (Exception xEx){
                    if(xEx.Message.Contains("Acceso denegado al Registro solicitado.")) {
                        continue;
                    } else {
                        throw xEx;
                    }
                }
            }            
            vReg.Close();            
        }
        private void frmCleanVBRegistry_Load(object sender,EventArgs e) {

        }        
    }
}
