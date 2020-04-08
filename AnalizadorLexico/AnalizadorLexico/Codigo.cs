using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class Codigo
    {
        Dictionary<string, List<int>> alfabeto = new Dictionary<string, List<int>>();
        string outPutClass = "";

        public void setAlfabeto(Dictionary<string, List<int>> alfabeto) {
            this.alfabeto = alfabeto;
        }

        public string getClass() {
            return outPutClass;
        }

        public void escribirClase() {
            outPutClass += "using System;\n" +
                           "using System.Collections;\n" +
                           "using System.Collections.Generic;\n" +
                           "using System.Collections.Generic;\n\n" +
                           "class Automata {\n" +
                           "static void Main(string[] args) {\n" +
                           "\tint estado = 1;\n";                        
        }

        public void terminarClase() {
            outPutClass += "\n}\n}";
        }
        public void escribirConjuntos(Dictionary<string, List<int>> alfabeto) {            
            for (int i = 0; i < alfabeto.Count; i++)
            {
                outPutClass += escribirLista(alfabeto.ElementAt(i));
            }
        }

        public string escribirLista(KeyValuePair<string, List<int>> elemento) {
            return "\tList<int> llave = new List<int> {" + string.Join(",", elemento.Value.ToArray()) + "};\n";
        }

        public void escribirInterfaz() {
            outPutClass += "\tstring path = @\"\";\n" +
                           "\tConsole.Writeline(\"Ingrese la direccion del archivo de entrada: \");\n" +
                           "\tpath += Console.ReadLine();\n" +
                           "\tstring cadena = System.IO.File.ReadAllText(path);\n" +
                           "\n\tfor(int i = 0; i < cadena.Length; i++) {\n";
        }

        public void terminarFor() {
            outPutClass += '}';
        }

        public void escribirSwitch() {
            outPutClass += "\nswitch(estado){";
        }

        public void terminarSwitch() {
            outPutClass += '}';
        }

        public void escribirCase(int num, List<Transicion> transicion) { }

        public void escribirIf() { }
    }
}
