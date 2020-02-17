namespace AnalizadorLexico
{
    partial class GUI
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ingresarArchivo = new System.Windows.Forms.Button();
            this.leerArchivo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(31, 58);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(198, 20);
            this.textBox1.TabIndex = 0;
            // 
            // ingresarArchivo
            // 
            this.ingresarArchivo.Location = new System.Drawing.Point(31, 84);
            this.ingresarArchivo.Name = "ingresarArchivo";
            this.ingresarArchivo.Size = new System.Drawing.Size(75, 41);
            this.ingresarArchivo.TabIndex = 1;
            this.ingresarArchivo.Text = "Ingresar Archivo";
            this.ingresarArchivo.UseVisualStyleBackColor = true;
            this.ingresarArchivo.Click += new System.EventHandler(this.ingresarArchivo_Click);
            // 
            // leerArchivo
            // 
            this.leerArchivo.Location = new System.Drawing.Point(154, 84);
            this.leerArchivo.Name = "leerArchivo";
            this.leerArchivo.Size = new System.Drawing.Size(75, 41);
            this.leerArchivo.TabIndex = 2;
            this.leerArchivo.Text = "Leer Archivo";
            this.leerArchivo.UseVisualStyleBackColor = true;
            this.leerArchivo.Click += new System.EventHandler(this.leerArchivo_Click);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 450);
            this.Controls.Add(this.leerArchivo);
            this.Controls.Add(this.ingresarArchivo);
            this.Controls.Add(this.textBox1);
            this.Name = "GUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ingresarArchivo;
        private System.Windows.Forms.Button leerArchivo;
    }
}

