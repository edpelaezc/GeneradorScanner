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
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.generarDFA = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(31, 58);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(452, 20);
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
            this.leerArchivo.Location = new System.Drawing.Point(112, 84);
            this.leerArchivo.Name = "leerArchivo";
            this.leerArchivo.Size = new System.Drawing.Size(75, 41);
            this.leerArchivo.TabIndex = 2;
            this.leerArchivo.Text = "Leer Archivo";
            this.leerArchivo.UseVisualStyleBackColor = true;
            this.leerArchivo.Click += new System.EventHandler(this.leerArchivo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "SELECCIONE ARCHIVO .txt";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(31, 291);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(452, 191);
            this.dataGridView1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 275);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "AUTÓMATA FINITO DETERMINISTA";
            // 
            // generarDFA
            // 
            this.generarDFA.Enabled = false;
            this.generarDFA.Location = new System.Drawing.Point(341, 151);
            this.generarDFA.Name = "generarDFA";
            this.generarDFA.Size = new System.Drawing.Size(142, 43);
            this.generarDFA.TabIndex = 6;
            this.generarDFA.Text = "GENERAR DFA";
            this.generarDFA.UseVisualStyleBackColor = true;
            this.generarDFA.Click += new System.EventHandler(this.generarDFA_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(31, 240);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(452, 20);
            this.textBox2.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "EXPRESIÓN REGULAR";
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 509);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.generarDFA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.leerArchivo);
            this.Controls.Add(this.ingresarArchivo);
            this.Controls.Add(this.textBox1);
            this.Name = "GUI";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ingresarArchivo;
        private System.Windows.Forms.Button leerArchivo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button generarDFA;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
    }
}

