using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AnalizadorLexico
{
    public partial class GUI : Form
    {
        OpenFileDialog openFile;
        StreamReader fileReader;
        string path = "";
        LinkedList<string> sets = new LinkedList<string>();


        public GUI()
        {
            InitializeComponent();
        }

        private void ingresarArchivo_Click(object sender, EventArgs e)
        {
            openFile = new OpenFileDialog();
            openFile.Title = "SELECCIONE EL ARCHIVO";
            openFile.Filter = "Archivos de texto(*.txt)|*.txt";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFile.FileName;
                path = openFile.FileName;
            }
        }

        private void leerArchivo_Click(object sender, EventArgs e)
        {
            string current = "";
            string line = "";
            fileReader = new StreamReader(path);
            line = fileReader.ReadLine();
            line = quitarEspacios(line);
            
            while (line != null)
            {
                if (line.ToUpper() == "SETS" | line.ToUpper() == "TOKENS" | line.ToUpper() == "ACTIONS") {
                    current = line;
                }

                if (current == "SETS") // metodo para leer los SETS
                {

                }
                else if (current == "TOKENS")
                {

                }
                else if (current == "ACTIONS")
                {

                }
                line = fileReader.ReadLine();
                line = quitarEspacios(line);
            }
        }

        public void leerSets() { 
        
        }

        public string quitarEspacios(string cadena) {
            string response = "";
            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i] != ' ')                
                    response = response + cadena[i];                
            }
            return response;
        }
    }
}
