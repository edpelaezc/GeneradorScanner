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
using System.Text.RegularExpressions;

namespace AnalizadorLexico
{
    public partial class GUI : Form
    {
        OpenFileDialog openFile;
        StreamReader fileReader;
        bool error = false;
        string path = "";
        LinkedList<string> sets = new LinkedList<string>();
        Dictionary<string, List<int>> alfabeto = new Dictionary<string, List<int>>();

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
            
            while (line != null && error == false)
            {
                if (line.ToUpper() == "SETS" | line.ToUpper() == "TOKENS" | line.ToUpper() == "ACTIONS") {
                    current = line;
                    line = fileReader.ReadLine();
                    line = quitarEspacios(line);
                }

                if (current == "SETS") // metodo para leer los SETS
                {
                    leerSets(line);
                }
                else if (current == "TOKENS")
                {

                }
                else if (current == "ACTIONS")
                {

                }
                line = fileReader.ReadLine();
                if (line != null){
                    line = quitarEspacios(line);
                }                
            }           
        }

        public void leerSets(string cadena) {
            string[] separadores = { ".." };
            int limiteInferior = 0;
            int limiteSuperior = 0;
            List<int> setAux = new List<int>();
            if (cadena != "") {
                string nombre = cadena.Substring(0, cadena.IndexOf('='));
                string[] conjuntos = cadena.Substring(cadena.IndexOf('=') + 1).Split('+');

                for (int i = 0; i < conjuntos.Length; i++)
                {
                    if (conjuntos[i].Contains("..")) { //si es un rango 
                        //obtener los rangos inferior y superior 
                        string[] limites = conjuntos[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                        if (limites[0].Contains("CHR")) //agregar rango que contenga funcion CHR
                        {
                            limiteInferior = validarCHR(limites[0]);
                            limiteSuperior = validarCHR(limites[1]);

                            if ((limiteInferior < limiteSuperior) && limiteInferior != 0 && limiteSuperior != 0)
                            {
                                for (int k = limiteInferior; k < limiteSuperior + 1; k++)
                                {
                                    setAux.Add(k);
                                }
                            }
                            else
                            {
                                error = true;
                                MessageBox.Show("ERROR DE FORMATO EN EL RANGO\n\t" + cadena);
                            }
                        }
                        else if (limites[0].Contains("CH") || limites[0].Contains("CR")) { // si está mal el formato de char
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO DE RANGO\n\t" + cadena);
                        }
                        else // agregar el rango al alfabeto
                        {
                            limiteInferior = validarLimite(limites[0]);
                            limiteSuperior = validarLimite(limites[1]);

                            if ((limiteInferior < limiteSuperior) && limiteInferior != 0 && limiteSuperior != 0)
                            {
                                for (int k = limiteInferior; k < limiteSuperior + 1; k++)
                                {
                                    setAux.Add(k);
                                }
                            }
                            else
                            {
                                error = true;
                                MessageBox.Show("ERROR DE FORMATO EN EL RANGO\n\t" + cadena);
                            }
                        }                      

                    }
                    else if (conjuntos[i].Contains(".")) { // esta mal el formato de rango 
                        error = true;
                        MessageBox.Show("ERROR DE FORMATO DE RANGO\n\t" + cadena);
                    }
                    else { // es un elemento individual 
                        int aux = validarLimite(conjuntos[i]);
                        if (aux != 0) {
                            setAux.Add(aux);
                        }
                        else {
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO\n\t" + cadena);
                        }           
                    }
                }
                //agregar elementos al diccionario que contiene el alfabeto
                if (setAux.Count != 0) {
                    alfabeto.Add(nombre, setAux);
                }                
            }            
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

        public int validarLimite(string cadena) {
            if (cadena.Length == 3)
            {
                if (cadena[0] == '\'' && cadena[2] == '\'') {
                    return (char)cadena[1];
                }
                else {
                    error = true;
                    return 0;
                }
            }
            else
            {
                error = true;
                return 0; //0 es el valor nulo en ascii
            }
        }

        public int validarCHR(string cadena) {
            if (cadena[3] == '(' && cadena[cadena.Length - 1] == ')')
            {
                int limiteCadena = cadena.IndexOf(')') - cadena.IndexOf('(');
                return (char)int.Parse((cadena.Substring(cadena.IndexOf('(') + 1, limiteCadena - 1)));
            }
            else
            {
                error = true;
                return 0;
            }
        }
    }
}
