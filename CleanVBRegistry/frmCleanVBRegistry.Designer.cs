namespace CleanVBRegistry {
    partial class frmCleanVBRegistry {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.button1 = new System.Windows.Forms.Button();
            this.dgvRegKey = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.chLibGalac = new System.Windows.Forms.CheckBox();
            this.chContabRpt = new System.Windows.Forms.CheckBox();
            this.chSaw = new System.Windows.Forms.CheckBox();
            this.txtLibGalac = new System.Windows.Forms.TextBox();
            this.txtContabRpt = new System.Windows.Forms.TextBox();
            this.txtSaw = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegKey)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Buscar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dgvRegKey
            // 
            this.dgvRegKey.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRegKey.Location = new System.Drawing.Point(7, 94);
            this.dgvRegKey.Name = "dgvRegKey";
            this.dgvRegKey.Size = new System.Drawing.Size(287, 252);
            this.dgvRegKey.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 55);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 27);
            this.button2.TabIndex = 2;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chLibGalac
            // 
            this.chLibGalac.AutoSize = true;
            this.chLibGalac.Location = new System.Drawing.Point(104, 17);
            this.chLibGalac.Name = "chLibGalac";
            this.chLibGalac.Size = new System.Drawing.Size(68, 17);
            this.chLibGalac.TabIndex = 3;
            this.chLibGalac.Text = "LibGalac";
            this.chLibGalac.UseVisualStyleBackColor = true;
            // 
            // chContabRpt
            // 
            this.chContabRpt.AutoSize = true;
            this.chContabRpt.Location = new System.Drawing.Point(104, 40);
            this.chContabRpt.Name = "chContabRpt";
            this.chContabRpt.Size = new System.Drawing.Size(77, 17);
            this.chContabRpt.TabIndex = 4;
            this.chContabRpt.Text = "ContabRpt";
            this.chContabRpt.UseVisualStyleBackColor = true;
            // 
            // chSaw
            // 
            this.chSaw.AutoSize = true;
            this.chSaw.Location = new System.Drawing.Point(104, 67);
            this.chSaw.Name = "chSaw";
            this.chSaw.Size = new System.Drawing.Size(47, 17);
            this.chSaw.TabIndex = 5;
            this.chSaw.Text = "Saw";
            this.chSaw.UseVisualStyleBackColor = true;
            // 
            // txtLibGalac
            // 
            this.txtLibGalac.Location = new System.Drawing.Point(180, 14);
            this.txtLibGalac.Name = "txtLibGalac";
            this.txtLibGalac.Size = new System.Drawing.Size(45, 20);
            this.txtLibGalac.TabIndex = 6;
            // 
            // txtContabRpt
            // 
            this.txtContabRpt.Location = new System.Drawing.Point(180, 39);
            this.txtContabRpt.Name = "txtContabRpt";
            this.txtContabRpt.Size = new System.Drawing.Size(45, 20);
            this.txtContabRpt.TabIndex = 7;
            // 
            // txtSaw
            // 
            this.txtSaw.Location = new System.Drawing.Point(180, 64);
            this.txtSaw.Name = "txtSaw";
            this.txtSaw.Size = new System.Drawing.Size(45, 20);
            this.txtSaw.TabIndex = 8;
            // 
            // frmCleanVBRegistry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 355);
            this.Controls.Add(this.txtSaw);
            this.Controls.Add(this.txtContabRpt);
            this.Controls.Add(this.txtLibGalac);
            this.Controls.Add(this.chSaw);
            this.Controls.Add(this.chContabRpt);
            this.Controls.Add(this.chLibGalac);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dgvRegKey);
            this.Controls.Add(this.button1);
            this.Name = "frmCleanVBRegistry";
            this.Text = "Clean VB Registry";
            this.Load += new System.EventHandler(this.frmCleanVBRegistry_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegKey)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dgvRegKey;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chLibGalac;
        private System.Windows.Forms.CheckBox chContabRpt;
        private System.Windows.Forms.CheckBox chSaw;
        private System.Windows.Forms.TextBox txtLibGalac;
        private System.Windows.Forms.TextBox txtContabRpt;
        private System.Windows.Forms.TextBox txtSaw;
    }
}