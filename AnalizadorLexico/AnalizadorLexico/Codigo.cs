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
        Dictionary<int, string> actions = new Dictionary<int, string>();
        Dictionary<string, int> errores = new Dictionary<string, int>();
        List<int> terminales = new List<int>();
        List<Estado> estados = new List<Estado>();
        string outPutClass = "";
        
        public void escribirActions() {            
            outPutClass += "\tDictionary<int, string> actions = new Dictionary<int, string>()\n" +
                           "\t{\n";
            for (int i = 0; i < actions.Count; i++)
            {
                KeyValuePair<int, string> aux = actions.ElementAt(i);
                if (i != actions.Count - 1)
                {
                    outPutClass += "\t\t{" + aux.Key.ToString() + ", \"" + aux.Value.Replace("'", "") + "\"},\n";
                }
                else
                {
                    outPutClass += "\t\t{" + aux.Key.ToString() + ", \"" + aux.Value.Replace("'", "") + "\"}\n";
                }
            }
            outPutClass += "\t};\n\n"; //terminar diccionario
        }

        public void setErrores(Dictionary<string, int> errores) {
            this.errores = errores; 
        }

        public void setEstados(List<Estado> estados) {
            this.estados = estados;
        }

        public void setAlfabeto(Dictionary<string, List<int>> alfabeto) {
            this.alfabeto = alfabeto;
        }

        public void setActions(Dictionary<string, List<KeyValuePair<int, string>>> original) {            
            for (int i = 0; i < original.Count; i++)
            {
                List<KeyValuePair<int, string>> auxiliar = original.ElementAt(i).Value;

                for (int j = 0; j < auxiliar.Count; j++)
                {
                    actions.Add(auxiliar[j].Key, auxiliar[j].Value);
                }
            }   
        }

        public string getClass() {
            return outPutClass;
        }

        public void escribirClase() {
            outPutClass += "using System;\n" +
                           "using System.IO;\n" +
                           "using System.Linq;\n" +
                           "using System.Collections;\n" +
                           "using System.Collections.Generic;\n\n" +
                           "class Automata {\n";                                          
        }

        public void escribirMain() { 
            outPutClass += "static void Main(string[] args) {\n" +
                           "\tint estado = 1;\n" +
                           "\tstring auxiliar = \"\";\n";
        }

        public void terminarMain() {
            outPutClass += "\n}";
        }

        public void terminarClase() {
            outPutClass += "\n}";
        }
        public void escribirConjuntos(Dictionary<string, List<int>> alfabeto) {            
            for (int i = 0; i < alfabeto.Count; i++)
            {
                outPutClass += escribirLista(alfabeto.ElementAt(i));
            }
        }

        public string escribirLista(KeyValuePair<string, List<int>> elemento) {
            return "\tstatic List<int> " + elemento.Key.ToString() + " = new List<int> {" + string.Join(",", elemento.Value.ToArray()) + "};\n";
        }

        public void escribirLista(List<int> elemento, string nombre)
        {
            outPutClass += "\tstatic List<int> " + nombre + " = new List<int> {" + string.Join(",", elemento.ToArray()) + "};\n";
        }

        public void escribirInterfaz() {
            outPutClass += "\tstring path = @\"\";\n" +
                           "\tConsole.WriteLine(\"Ingrese la direccion del archivo de entrada: \");\n" +
                           "\tpath += Console.ReadLine();\n" +
                           "\tstring cadena = System.IO.File.ReadAllText(path);\n" +
                           "\n\tfor(int i = 0; i < cadena.Length; i++) {\n";
        }

        public void terminarFor() {
            outPutClass += "\t}\n\n" +
                "\tConsole.WriteLine(auxiliar + \", TOKEN: \" + obtenerToken(estado, auxiliar));\n" +
                "\tConsole.WriteLine(\"\\nTERMINADO\");\n" +
                "\tConsole.ReadLine();\n";
        }

        public void escribirSwitch() {
            outPutClass += "\t\tswitch(estado){\n";
        }

        public void terminarSwitch() {
            outPutClass += "\t\t}\n";
        }

        public void escribirCase(int num, List<Transicion> transicion) {
            string condicion = "";
            outPutClass += "\t\t\tcase " + num.ToString() + ":\n";            
            
            if (transicion.Count != 0) // si el estado tiene transiciones
            {
                int actual = transicion[0].origen.numero;
                for (int i = 0; i < transicion.Count; i++)
                {
                    if (alfabeto.Keys.Contains(transicion[i].simbolo))
                    {
                        condicion = transicion[i].simbolo.ToString() + ".Contains(cadena[i])";
                    }
                    else
                    {
                        condicion = "cadena[i] == " + (int)Convert.ToChar(transicion[i].simbolo);
                    }

                    //manejar formato de condicion
                    if (i == 0)
                    {
                        outPutClass += "\t\t\t\tif(" + condicion + "){\n" +
                                       "\t\t\t\t\testado = " + transicion[i].destino.numero.ToString() + ";\n" +
                                       "\t\t\t\t\tauxiliar += cadena[i];\n\t\t\t\t}\n";
                    }
                    else
                    {
                        outPutClass += "\t\t\t\telse if(" + condicion + "){\n" +
                                       "\t\t\t\t\testado = " + transicion[i].destino.numero.ToString() + ";\n" +
                                       "\t\t\t\t\tauxiliar += cadena[i];\n\t\t\t\t}\n";
                    }

                    if (i == transicion.Count - 1)
                    {
                        if (actual == 1)
                        {
                            //validacion de espacios 
                            outPutClass += "\t\t\t\telse if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){\n" +
                                           "\t\t\t\t\t//obviar los espacios\n" +
                                           "\t\t\t\t}\n";
                        }
                        else
                        {
                            //validacion de espacios 
                            outPutClass += "\t\t\t\telse if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){\n" +
                                           "\t\t\t\t\tConsole.WriteLine(auxiliar + \", TOKEN: \" + obtenerToken(estado, auxiliar));\n" +
                                           "\t\t\t\t\tauxiliar = \"\";\n" +
                                           "\t\t\t\t\testado = 1;\n" +
                                           "\t\t\t\t}\n";
                        }
                    }
                }

                //agregar else 
                //si es el estado inicial 
                if (transicion[0].origen.numero.Equals(1))
                {
                    outPutClass += "\t\t\t\telse{\n" +
                                   "\t\t\t\t\tConsole.WriteLine(cadena[i] + \", no. de token: " + errores.ElementAt(0).Value + "\");\n" +
                                   "\t\t\t\t\testado = 1;\n" +
                                   "\t\t\t\t}\n";
                }
                else
                {
                    outPutClass += "\t\t\t\telse{\n" +
                   "\t\t\t\t\tif(validacion(cadena[i]) == false){\n" +
                   "\t\t\t\t\t\tauxiliar += cadena[i];\n" +
                   "\t\t\t\t\t\tConsole.WriteLine(auxiliar + \", no. de token: " + errores.ElementAt(0).Value + "\");\n" +
                   "\t\t\t\t\t\tauxiliar = \"\";\n" +
                   "\t\t\t\t\t\testado = 1;\n" +
                   "\t\t\t\t\t}\n" +
                   "\t\t\t\t\telse{\n" +
                   "\t\t\t\t\t\t//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado\n" +
                   "\t\t\t\t\t\tConsole.WriteLine(auxiliar + \", TOKEN: \" + obtenerToken(estado, auxiliar));\n" +
                   "\t\t\t\t\t\testado = 1;\n" +
                   "\t\t\t\t\t\ti--;\n" +
                   "\t\t\t\t\t\tauxiliar = \"\";\n" +
                   "\t\t\t\t\t}\n" +
                   "\t\t\t\t}\n";
                }

            }
            else
            {
                // si el estado no tiene transiciones
                //validacion de espacios 
                outPutClass += "\t\t\t\t\tif(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){\n" +
                               "\t\t\t\t\t\tConsole.WriteLine(auxiliar + \", TOKEN: \" + obtenerToken(estado, auxiliar));\n" +
                               "\t\t\t\t\t\tauxiliar = \"\";\n" +
                               "\t\t\t\t\t\testado = 1;\n" +
                               "\t\t\t\t\t}\n";                
                outPutClass +=
               "\t\t\t\t\t//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado\n" +
               "\t\t\t\t\telse{\n" +
               "\t\t\t\t\t\tConsole.WriteLine(auxiliar + \", TOKEN: \" + obtenerToken(estado, auxiliar));\n" +
               "\t\t\t\t\t\testado = 1;\n" +
               "\t\t\t\t\t\ti--;\n" +
               "\t\t\t\t\t\tauxiliar = \"\";\n" +
               "\t\t\t\t\t}\n";                   
            }


            
            outPutClass += "\t\t\tbreak;\n";
        }

        public void setTerminles(List<int> sTerminales) {
            terminales = sTerminales;
        }

        public void escribirValidacionConjunto() {
            string condicion = "";
            outPutClass += "\n\n\tstatic bool validacion(char cadena){\n" +
                           "\t\tbool response = false;\n" +
                           "\t\tif(";
            for (int i = 0; i < alfabeto.Count; i++)
            {
                condicion = alfabeto.ElementAt(i).Key.ToString() + ".Contains(cadena)";
                if (i == 0)
                {
                    outPutClass += condicion;
                }
                else
                {
                    outPutClass += " || " + condicion;
                }
            }


            condicion = " || TERMINALES.Contains(cadena)";
            outPutClass += condicion;

            outPutClass += "){\n";
            outPutClass += "\t\t\tresponse = true;\n";
            outPutClass += "\t\t}\n" +
                           "\t\treturn response;\n"; //termina if 
            outPutClass += "\t}\n"; //termina el procedimiento
        }

        public void escribirCaseToken() {
            outPutClass += "\n\n\n\tstatic int obtenerToken(int estado, string auxiliar){\n";
            escribirActions();
            outPutClass += "\t\tint response = " + errores.ElementAt(0).Value.ToString() + ";\n";
            //compara si auxiliar pertenece a las actions, si no: evalua los estados.
            outPutClass += "\t\tif(actions.Values.Contains(auxiliar.ToUpper())){\n" +
                           "\t\t\tfor(int i = 0; i < actions.Count; i++){\n" +
                           "\t\t\t\tif(actions.ElementAt(i).Value.Equals(auxiliar.ToUpper())){\n" +
                           "\t\t\t\t\tresponse = actions.ElementAt(i).Key;\n\t\t\t\t}\n" +
                           "\t\t\t}\n" +
                           "\t\t} else {\n";


            outPutClass += "\t\t\tswitch(estado){\n"; //empieza switch
            for (int i = 0; i < estados.Count; i++)
            {
                if (estados[i].aceptacion)
                {
                    outPutClass += "\t\t\t\tcase " + estados[i].numero.ToString() + ":\n";
                    if (estados[i].token.Count == 1)
                    {
                        KeyValuePair<List<string>, int> auxiliar = estados[i].token[0];
                        outPutClass += "\t\t\t\t\tresponse = " + auxiliar.Value.ToString() + "; break;\n";
                    }
                    else
                    {
                        for (int j = 0; j < estados[i].token.Count; j++)
                        {
                            KeyValuePair<List<string>, int> auxiliar = estados[i].token[j];
                            if (!contieneSet(auxiliar.Key)) // si no contiene sets
                            {
                                outPutClass += "\t\t\t\t\tif(auxiliar == \"" + concatenar(auxiliar.Key) + "\"){\n";
                                outPutClass += "\t\t\t\t\t\tresponse = " + auxiliar.Value.ToString() + ";\n";
                                outPutClass += "\t\t\t\t\t}\n";                                
                            }
                        }
                        outPutClass += "\t\t\t\t\tbreak;\n";
                    }
                }
            }
            
            outPutClass += "\t\t\t}\n"; //termina switch
            outPutClass += "\t\t}\n";
            outPutClass += "\t\treturn response; \n";
            outPutClass += "\t}\n"; // termina procedimiento
        }

        public bool contieneSet(List<string> auxiliar)
        {
            bool response = false;
            List<string> keys = alfabeto.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                for (int j = 0; j < auxiliar.Count; j++)
                {
                    if (auxiliar[j].Contains(keys[i]))
                    {
                        response = true;
                    }
                }
            }
            return response;
        }

        public string concatenar(List<string> token) {
            string response = "";
            for (int i = 0; i < token.Count; i++)
            {
                response += token[i];
            }
            return response; 
        }
    }
}
