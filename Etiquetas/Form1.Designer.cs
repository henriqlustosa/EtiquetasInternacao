namespace Etiquetas
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btImprimir = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txbRh = new System.Windows.Forms.TextBox();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblError = new System.Windows.Forms.Label();
            this.lbTitulo = new System.Windows.Forms.Label();
            this.rbEtiqueta_6 = new System.Windows.Forms.RadioButton();
            this.rbEtiqueta_8 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btImprimir
            // 
            this.btImprimir.Location = new System.Drawing.Point(192, 216);
            this.btImprimir.Name = "btImprimir";
            this.btImprimir.Size = new System.Drawing.Size(75, 23);
            this.btImprimir.TabIndex = 5;
            this.btImprimir.Text = "Imprimir";
            this.btImprimir.UseVisualStyleBackColor = true;
            this.btImprimir.Click += new System.EventHandler(this.btImprimir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Informe o RH";
            // 
            // txbRh
            // 
            this.txbRh.Location = new System.Drawing.Point(124, 74);
            this.txbRh.Name = "txbRh";
            this.txbRh.Size = new System.Drawing.Size(143, 20);
            this.txbRh.TabIndex = 1;
            this.txbRh.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txbRh_KeyPress);
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(139, 157);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 6;
            // 
            // lbTitulo
            // 
            this.lbTitulo.AutoSize = true;
            this.lbTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitulo.Location = new System.Drawing.Point(104, 22);
            this.lbTitulo.Name = "lbTitulo";
            this.lbTitulo.Size = new System.Drawing.Size(242, 20);
            this.lbTitulo.TabIndex = 0;
            this.lbTitulo.Text = "Sistema de Etiquetas Internação";
            // 
            // rbEtiqueta_6
            // 
            this.rbEtiqueta_6.AutoSize = true;
            this.rbEtiqueta_6.Location = new System.Drawing.Point(291, 75);
            this.rbEtiqueta_6.Name = "rbEtiqueta_6";
            this.rbEtiqueta_6.Size = new System.Drawing.Size(77, 17);
            this.rbEtiqueta_6.TabIndex = 8;
            this.rbEtiqueta_6.TabStop = true;
            this.rbEtiqueta_6.Text = "6 etiquetas";
            this.rbEtiqueta_6.UseVisualStyleBackColor = true;
            // 
            // rbEtiqueta_8
            // 
            this.rbEtiqueta_8.AutoSize = true;
            this.rbEtiqueta_8.Location = new System.Drawing.Point(384, 77);
            this.rbEtiqueta_8.Name = "rbEtiqueta_8";
            this.rbEtiqueta_8.Size = new System.Drawing.Size(77, 17);
            this.rbEtiqueta_8.TabIndex = 9;
            this.rbEtiqueta_8.TabStop = true;
            this.rbEtiqueta_8.Text = "8 etiquetas";
            this.rbEtiqueta_8.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 269);
            this.Controls.Add(this.rbEtiqueta_8);
            this.Controls.Add(this.rbEtiqueta_6);
            this.Controls.Add(this.lbTitulo);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbRh);
            this.Controls.Add(this.btImprimir);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btImprimir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbRh;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lbTitulo;
        private System.Windows.Forms.RadioButton rbEtiqueta_6;
        private System.Windows.Forms.RadioButton rbEtiqueta_8;

    }
}

