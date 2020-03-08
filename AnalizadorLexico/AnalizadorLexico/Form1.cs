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
        //estructuras para guardar los datos del archivo 
        Dictionary<string, List<int>> alfabeto = new Dictionary<string, List<int>>();
        Dictionary<string, List<string>> tokens = new Dictionary<string, List<string>>();
        Dictionary<int, string> actions = new Dictionary<int, string>();
        Dictionary<string, int> errores = new Dictionary<string, int>();
        Dictionary<int, List<int>> follow = new Dictionary<int, List<int>>();
        List<string> auxActions = new List<string>();

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
            error = false;
            string current = "";
            string line = "";
            string archivo = "";
            bool contieneTokens = false;
            if (path != "")
            {
                fileReader = new StreamReader(path);
                archivo = fileReader.ReadToEnd();
                contieneTokens = archivo.IndexOf("tokens", StringComparison.OrdinalIgnoreCase) >= 0;
                fileReader = new StreamReader(path);
            }
            else
            {
                error = true;
                path = "";
                openFile = new OpenFileDialog();
                fileReader = null;
                MessageBox.Show("TIENE QUE INGRESAR UN ARCHIVO");
            }

            //validar si contiene la sección de TOKENS
            if (!contieneTokens)
            {
                error = true;
                MessageBox.Show("EL ARCHIVO NO CONTIENE LA SECCIÓN DE \"TOKENS\"\n" + archivo);
            }

            while (line != null && error == false) //CICLO PRINCIPAL DE LECTURA
            {
                //comparaciones de encabezados
                if (line.ToUpper().Contains("SETS")  | line.ToUpper().Contains("TOKENS") | line.ToUpper().Contains("ACTIONS")) {
                    current = line.ToUpper();
                    line = fileReader.ReadLine();

                    if (line != null && current != "TOKENS") {
                        line = quitarEspacios(line);
                    }

                    if (current.Trim() == "ACTIONS")
                    {
                        if (line.ToUpper().Trim() != "RESERVADAS()")
                        {
                            alfabeto = new Dictionary<string, List<int>>();
                            tokens = new Dictionary<string, List<string>>();
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO\n\t" + line);                            
                        }
                        else
                        {
                            line = fileReader.ReadLine();
                            line = quitarEspacios(line);
                            if (line.Trim() != "{")
                            {
                                alfabeto = new Dictionary<string, List<int>>();
                                tokens = new Dictionary<string, List<string>>();
                                error = true;
                                MessageBox.Show("ERROR DE FORMATO, NO CONTIENE \'{\' DESPUES DE \"RESERVADAS()\"\n" + archivo);
                            }
                            else
                            {                                
                                line = fileReader.ReadLine();
                                line = quitarEspacios(line);
                                //validar que contenga llave cerrada
                                while (line.Trim() != "}" && !line.ToUpper().Contains("ERROR"))
                                {
                                    auxActions.Add(line);
                                    line = fileReader.ReadLine();
                                    line = quitarEspacios(line);
                                }

                                if (line.Trim() != "}")
                                {
                                    alfabeto = new Dictionary<string, List<int>>();
                                    tokens = new Dictionary<string, List<string>>();
                                    auxActions = new List<string>();
                                    path = "";
                                    error = true;
                                    MessageBox.Show("ERROR DE FORMATO, NO CONTIENE \'}\'  DESPUES DE \"RESERVADAS()\"\n" + archivo);
                                }
                                else
                                {
                                    leerActions(auxActions);
                                }
                            }
                        }
                    }
                }

                if (current.Trim() == "SETS") // metodo para leer los. SETS
                {
                    leerSets(line);                    
                }
                else if (current.Trim() == "TOKENS")
                {
                    leerTokens(line);
                }
                else if (line.Trim().ToUpper().Contains("ERROR"))
                {
                    leerError(line);
                }
                
                line = fileReader.ReadLine();
                
                if (line != null && current != "TOKENS") { 
                    line = quitarEspacios(line); 
                }
            }

            if (line == null)
            {
                generarDFA.Enabled = true;
                MessageBox.Show("ARCHIVO LEÍDO CON ÉXITO");
            }

            if (error == true)
            {
                textBox1.Text = "";
                path = "";
            }
        }

        public void leerSets(string cadena) {
            string[] separadores = { ".." };
            int limiteInferior = 0;
            int limiteSuperior = 0;

            List<int> setAux = new List<int>();
            if (cadena != "") {
                if (validarIgual(cadena))
                {
                    string nombre = cadena.Substring(0, cadena.IndexOf('='));
                    string[] conjuntos = cadena.Substring(cadena.IndexOf('=') + 1).Split('+');

                    for (int i = 0; i < conjuntos.Length; i++)
                    {
                        if (conjuntos[i].Contains(".."))
                        { //si es un rango 
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
                                    alfabeto = new Dictionary<string, List<int>>();
                                    error = true;
                                    MessageBox.Show("ERROR DE FORMATO EN EL RANGO\n\t" + cadena);
                                }
                            }
                            else if (limites[0].Contains("CH") || limites[0].Contains("CR"))
                            { // si está mal el formato de char
                                alfabeto = new Dictionary<string, List<int>>();
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
                                    alfabeto = new Dictionary<string, List<int>>();
                                    error = true;
                                    MessageBox.Show("ERROR DE FORMATO EN EL RANGO\n\t" + cadena);
                                }
                            }

                        }
                        else if (conjuntos[i].Contains("."))
                        { // esta mal el formato de rango 
                            alfabeto = new Dictionary<string, List<int>>();
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO DE RANGO\n\t" + cadena);
                        }
                        else
                        { // es un elemento individual 
                            int aux = validarLimite(conjuntos[i]);
                            if (aux != 0)
                            {
                                setAux.Add(aux);
                            }
                            else
                            {
                                alfabeto = new Dictionary<string, List<int>>();
                                error = true;
                                MessageBox.Show("ERROR DE FORMATO\n\t" + cadena);
                            }
                        }
                    }
                    //agregar elementos al diccionario que contiene el alfabeto
                    if (setAux.Count != 0)
                    {
                        alfabeto.Add(nombre, setAux);
                    }
                }// SI LA CADENA NO TIENE FORMATO ADECUADO
                else
                {
                    alfabeto = new Dictionary<string, List<int>>();
                    error = true;
                    MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\'\n\t" + cadena);
                }                            
            }            
        }

        public bool validarIgual(string cadena) {
            Console.WriteLine(cadena.IndexOf('='));
            return cadena.IndexOf('=') != -1;
        }

        public string quitarEspacios(string cadena) {            
            string response = "";
            cadena = cadena.Replace('\t'.ToString(), "");
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

        /// <summary>
        /// Valida que el rango de charsets esté correctamente escrito 
        /// </summary>
        /// <param name="cadena">Límites superior o inferior del rango de charset</param>
        /// <returns></returns>
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

        /// <summary>
        /// Validaciones de los tokens.
        /// </summary>
        /// <param name="cadena">Linea del archivo de pruebas</param>
        public void leerTokens(string cadena) {
            if (cadena != "")
            {
                if (validarIgual(cadena))
                {
                    string nombre = cadena.Substring(0, cadena.IndexOf('='));
                    string auxNombre = quitarEspacios(nombre);
                    nombre = nombre.ToLower();                    
                    bool contieneToken = auxNombre.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0;                    

                    if (contieneToken)
                    {
                        bool tokenRechazado = false;                        
                        int resultado = 0;
                        int limiteCadena = cadena.IndexOf('=') - cadena.ToLower().IndexOf('n');
                        bool succes = int.TryParse(nombre.Substring(nombre.IndexOf('n') + 1, limiteCadena - 1), out resultado);

                        if (succes) { //el nombre de token es valido
                            string tokenAux = cadena.Substring(cadena.IndexOf('=') + 1);
                            //analizar la expresión 
                            List<string> keysIn = alfabeto.Keys.ToList();

                            for (int i = 0; i < keysIn.Count; i++) //ver si tiene los tokens concatenados
                            {
                                for (int j = 0; j < keysIn.Count; j++)
                                {
                                    if (tokenAux.Contains(keysIn[i] + keysIn[j]))
                                    {
                                        alfabeto = new Dictionary<string, List<int>>();
                                        tokens = new Dictionary<string, List<string>>();
                                        error = true;
                                        MessageBox.Show("ERROR DE FORMATO, LAS CLAVES ESTÁN CONCATENADAS\n\t" + cadena);                                        
                                        tokenRechazado = true;
                                    }
                                }
                            }                            

                            
                            if (!tokenRechazado)//el token paso validaciones generales
                            {
                                //leer token y procesarlo 
                                List<string> succesToken = obtenerToken(tokenAux);

                                if (succesToken.Count == 0) {
                                    alfabeto = new Dictionary<string, List<int>>();
                                    tokens = new Dictionary<string, List<string>>();
                                    MessageBox.Show("ERROR EN EL FORMATO DEL TOKEN\n\t" + cadena);
                                    error = true;
                                }
                                else
                                {
                                    //hay que comprobar si el nombre ya existe o no en el diccionario
                                    try
                                    {
                                        tokens.Add(auxNombre, succesToken);
                                    }
                                    catch (ArgumentException)
                                    {
                                        alfabeto = new Dictionary<string, List<int>>();
                                        tokens = new Dictionary<string, List<string>>();
                                        error = true;
                                        MessageBox.Show("EL TOKEN YA EXISTE\n\t" + cadena);                                        
                                    }
                                    Console.WriteLine("NOMBRE VALIDO");
                                }
                            }
                        }//error de formato en el token
                        else {
                            alfabeto = new Dictionary<string, List<int>>();
                            tokens = new Dictionary<string, List<string>>();
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO, NOMBRE DE TOKEN INVÁLIDO\n\t" + cadena);
                        }
                    }
                    else
                    {
                        alfabeto = new Dictionary<string, List<int>>();
                        tokens = new Dictionary<string, List<string>>();
                        error = true;
                        MessageBox.Show("ERROR DE FORMATO, NO CONTIENE LA PALARA \'TOKEN\'\n\t" + cadena);
                    }

                }//SI LA CADENA NO TIENE FORMATO ADECUADO 
                else
                {
                    alfabeto = new Dictionary<string, List<int>>();
                    tokens = new Dictionary<string, List<string>>();
                    error = true;
                    MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\'\n\t" + cadena);
                }

            }
        }

        /// <summary>
        /// Recibe la cadena del token entera desde el simbolo '=' hasta el final de la cadena. 
        /// </summary>
        /// <param name="cadena">Token completo</param>
        public List<string> obtenerToken(string cadena) {
            cadena = cadena.Trim();
            List<string> response = new List<string>();
            string aux = "";
            
            for (int j = 0; j < cadena.Length; j++)
            {
                if (cadena[j] == ' ')
                {
                    if (aux != "")
                    {
                        response.Add(aux);
                    }
                    Console.WriteLine(aux);
                    if (!alfabeto.Keys.ToList().Contains(aux) && aux != "" && aux != ")" && aux != "(" && aux != "|" && aux != "?")
                    {
                        aux = "";
                        j = cadena.Length;
                    }
                    else
                    {
                        if (cadena[j + 1] != '?' && cadena[j + 1] != '+' && cadena[j + 1] != '*' && cadena[j + 1] != ')' && cadena[j + 1] != '|' && cadena[j - 1] != '(' && cadena[j - 1] != '|')
                        {
                            response.Add(".");
                        }
                        aux = "";
                    }
                }
                else if (cadena[j] == '\'')
                {
                    if (cadena[j + 2] != '\'') //si no está cerrada la comilla simple sale del ciclo y devuelve token invalido
                    {
                        aux = "";
                        j = cadena.Length;
                    }
                    else
                    {
                        response.Add(cadena[j + 1].ToString());
                        aux = "";
                        j += 2;
                    }
                }
                else if (cadena[j] == '(' || cadena[j] == ')' || cadena[j] == '*' || cadena[j] == '+' || cadena[j] == '|' || cadena[j] == '?')
                {
                    if (aux != "")
                    {
                        response.Add(aux);
                        aux = "";
                    }
                    response.Add(cadena[j].ToString());
                }
                else
                {
                    aux += cadena[j];
                }
            }


            if (aux != "")
            {
                response.Add(aux);
            }            


            return response;
        }


        public void leerActions(List<string> reservadas) {
            if (reservadas.Count == 0)
            {
                error = true;
            }
            else // las actions no contienen error de estructura
            {
                for (int i = 0; i < reservadas.Count; i++)
                {
                    //verificar errores de formato
                    int resultado;
                    string aux = reservadas[i];
                    aux = aux.Trim();

                    if (validarIgual(aux))
                    {
                        string numero = aux.Substring(0, aux.IndexOf('='));
                        bool succes = int.TryParse(numero, out resultado);

                        if (succes) // si el numero de action es correcto
                        {
                            string palabra = aux.Substring(aux.IndexOf('=') + 1);

                            if (palabra[0] == '\'' && palabra[palabra.Length - 1] == '\'')
                            {
                                try
                                {
                                    palabra = palabra.Replace("\'", "");
                                    actions.Add(int.Parse(numero), palabra);
                                }
                                catch (ArgumentException)
                                {
                                    alfabeto = new Dictionary<string, List<int>>();
                                    tokens = new Dictionary<string, List<string>>();
                                    auxActions = new List<string>();
                                    actions = new Dictionary<int, string>();
                                    error = true;
                                    MessageBox.Show("LA PALABRA RESERVADA YA EXISTE\n\t" + aux);
                                }
                            }
                            else
                            {
                                alfabeto = new Dictionary<string, List<int>>();
                                tokens = new Dictionary<string, List<string>>();
                                actions = new Dictionary<int, string>();
                                auxActions = new List<string>();
                                i = reservadas.Count;
                                error = true;
                                MessageBox.Show("ERROR DE FORMATO, FALTA \'\n\t" + aux);
                            }
                        }
                        else
                        {
                            alfabeto = new Dictionary<string, List<int>>();
                            tokens = new Dictionary<string, List<string>>();
                            actions = new Dictionary<int, string>();
                            auxActions = new List<string>();
                            i = reservadas.Count;
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\'\n\t" + aux);
                        }
                    }
                    else //ERROR EN EL FORMATO
                    {
                        alfabeto = new Dictionary<string, List<int>>();
                        tokens = new Dictionary<string, List<string>>();
                        actions = new Dictionary<int, string>();
                        auxActions = new List<string>();
                        i = reservadas.Count;
                        error = true;
                        MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\'\n\t" + aux);
                    }
                }                
            }
        }


        public void leerError(string cadena) {
            if (validarIgual(cadena))
            {
                cadena = cadena.Trim();
                int numero = 0;
                string nombre = cadena.Substring(0, cadena.IndexOf('='));
                bool succes = int.TryParse(cadena.Substring(cadena.IndexOf('=') + 1), out numero);

                if (succes)
                {
                    try
                    {
                        errores.Add(nombre, numero);
                    }
                    catch (ArgumentException)
                    {
                        alfabeto = new Dictionary<string, List<int>>();
                        tokens = new Dictionary<string, List<string>>();
                        auxActions = new List<string>();
                        actions = new Dictionary<int, string>();
                        errores = new Dictionary<string, int>();
                        error = true;
                        MessageBox.Show("EL ERROR YA EXISTE RESERVADA YA EXISTE\n\t" + cadena);
                    }
                }
                else
                {
                    alfabeto = new Dictionary<string, List<int>>();
                    tokens = new Dictionary<string, List<string>>();
                    auxActions = new List<string>();
                    actions = new Dictionary<int, string>();
                    errores = new Dictionary<string, int>();
                    error = true;
                    MessageBox.Show("ERROR DE FORMATO\n\t" + cadena);
                }
            }
            else //ERROR EN EL FORMATO
            {
                alfabeto = new Dictionary<string, List<int>>();
                tokens = new Dictionary<string, List<string>>();
                auxActions = new List<string>();
                actions = new Dictionary<int, string>();
                errores = new Dictionary<string, int>();
                error = true;
                MessageBox.Show("ERROR DE FORMATO, NO TIENE EL SINGO \'=\'\n\t" + cadena);
            }
        }

        private void generarDFA_Click(object sender, EventArgs e)
        {
            DFA funcionesDFA = new DFA();
            Console.WriteLine("adasf");
            //concatenar tokens para generar expresion regular
            string expresion = "";            
            List<List<string>> allTokens = tokens.Values.ToList();
            
            for (int i = 0; i < allTokens.Count; i++)
            {
                if (i == allTokens.Count - 1)
                {
                    expresion += '(';
                    List<string> aux = allTokens[i];
                    for (int j = 0; j < allTokens[i].Count; j++)
                    {
                        expresion += aux[j];
                    }
                    expresion += ')';
                    expresion += ".#";                                      

                }
                else
                {
                    expresion += '(';
                    List<string> aux = allTokens[i];
                    for (int j = 0; j < allTokens[i].Count; j++)
                    {
                        expresion += aux[j];
                    }
                    expresion += ')';
                    expresion += '|';
                }
            }

            textBox2.Text = expresion;

            List<string> final = new List<string>();
            final.Add("#");
            allTokens.Add(final);
            //transformar a postfijo todos los tokens
            for (int i = 0; i < allTokens.Count; i++)
            {
                allTokens[i] = funcionesDFA.transformarPostfijo(allTokens[i]);
            }

            //obtener los token en sus respectivos árboles de expresión
            List<Nodo> nodos = new List<Nodo>();
            for (int i = 0; i < allTokens.Count; i++)
            {
                if (allTokens[i].Count != 1)
                {                    
                    if (!allTokens[i].Contains("+") && !allTokens[i].Contains("*") && !allTokens[i].Contains("?") && !allTokens[i].Contains("|"))
                    {
                        Nodo concatenado = new Nodo();
                        List<string> auxiliar = allTokens[i];
                        string cadena = "";
                        for (int j = 0; j < auxiliar.Count; j++)
                        {
                            cadena += auxiliar[j];
                        }

                        concatenado.valor = cadena;
                        nodos.Add(concatenado);
                    }
                    else
                    {
                        nodos.Add(funcionesDFA.obtenerArbol(allTokens[i]));
                    }
                }
                else //si son solo símbolos
                {
                    Nodo aux = new Nodo();
                    List<string> auxList = allTokens[i];
                    aux.valor = auxList[0];
                    nodos.Add(aux);
                }    
            }

            List<Nodo> allNodes = new List<Nodo>();
            //union de los nodos 
            //raiz del arbol 
            Nodo concat = new Nodo();
            concat.valor = ".";
            for (int i = 0; i < nodos.Count; i++)
            {
                if (i == nodos.Count - 2)
                {                    
                    allNodes.Add(nodos[i]);
                    allNodes.Add(concat);
                    i++;
                    allNodes.Add(nodos[i]);
                }
                else
                {
                    allNodes.Add(nodos[i]);
                    Nodo orNode = new Nodo();
                    orNode.valor = "|";
                    allNodes.Add(orNode);
                }
            }
            
            //calcular first, last, follow haciendo uso de recorrido postfijo en el árbol.
            Nodo raiz = funcionesDFA.obtenerArbolCompleto(allNodes);
            funcionesDFA.calcularNulabilidad(raiz);
            funcionesDFA.contarNodosHoja(raiz);
            funcionesDFA.first(raiz);
            funcionesDFA.last(raiz);
            funcionesDFA.inicializarDiccionario();
            funcionesDFA.calcularFollow(raiz);            
            follow = funcionesDFA.getFollow();
        }





    }
}
