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
        int cont = 0;
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
                    cont++;

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
                            cont++;
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
                                cont++;
                                line = quitarEspacios(line);
                                //validar que contenga llave cerrada
                                while (line.Trim() != "}" && !line.ToUpper().Contains("ERROR"))
                                {
                                    auxActions.Add(line);
                                    line = fileReader.ReadLine();
                                    cont++;
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
                cont++;

                if (line != null && current != "TOKENS") { 
                    line = quitarEspacios(line); 
                }
            }

            if (line == null)
            {
                if (errores.Count == 0)
                {
                    error = true;
                    MessageBox.Show("EL ARCHIVO NO CONTIENE LA SECCIÓN DE ERRORES");
                }
                else
                {
                    generarDFA.Enabled = true;
                    MessageBox.Show("ARCHIVO LEÍDO CON ÉXITO");
                }
            }

            if (error == true)
            {
                textBox1.Text = "";
                path = "";
            }

            if (!error)
            {
                //mostrar resultados
                //mostrar SETS                          
                for (int i = 0; i < alfabeto.Count; i++)
                {
                    KeyValuePair<string, List<int>> valor = alfabeto.ElementAt(i);
                    listBox1.Items.Add(valor.Key);
                    listBox1.Items.Add("\t" + mostrar(valor.Value));
                }

                //mostrar tokens
                listBox2.Items.Add("TOKENS");               
                for (int i = 0; i < tokens.Count; i++)
                {
                    KeyValuePair<string, List<string>> valor = tokens.ElementAt(i);
                    listBox2.Items.Add("\t" + valor.Key + "   = "  + string.Join("", valor.Value.ToArray()));
                }

                //mostrar actions
                listBox2.Items.Add("ACTIONS");
                for (int i = 0; i < actions.Count; i++)
                {
                    KeyValuePair<int, string> valor = actions.ElementAt(i);
                    listBox2.Items.Add("\t" + valor.Key + "   = " +  "'" + valor.Value + "'");
                }

                //mostrar errores
                listBox2.Items.Add("ERROR");
                for (int i = 0; i < errores.Count; i++)
                {
                    KeyValuePair<string, int> valor = errores.ElementAt(i);
                    listBox2.Items.Add("\t" + valor.Key + "   = " + valor.Value);
                }


            }



        }

        public void leerSets(string cadena) {
            string[] separadores = { ".." };
            int limiteInferior = 0;
            int limiteSuperior = 0;

            List<int> setAux = new List<int>();
            if (cadena.Trim() != "") {
                if (validarIgual(cadena))
                {
                    string nombre = cadena.Substring(0, cadena.IndexOf('='));
                    string nombreAux = cadena.Substring(cadena.IndexOf('=') + 1);
                    nombreAux = nombreAux.Trim();
                    string[] conjuntos = nombreAux.Split('+');

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
                                    MessageBox.Show("ERROR DE FORMATO EN LA FUNCION CHR, LINEA No." + cont + "\n\t" + cadena);
                                }
                            }
                            else if (limites[0].Contains("CH") || limites[0].Contains("CR"))
                            { // si está mal el formato de char
                                alfabeto = new Dictionary<string, List<int>>();
                                error = true;
                                MessageBox.Show("FUNCION CHR MAL ESCRITA, LINEA No." + cont + "\n\t" + cadena);
                            }
                            else // agregar el rango al alfabeto
                            {
                                limiteInferior = validarLimite(limites[0].Trim());
                                limiteSuperior = validarLimite(limites[1].Trim());

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
                                    MessageBox.Show("ERROR DE FORMATO EN EL RANGO, LINEA No." + cont + "\n\t" + cadena);
                                }
                            }

                        }
                        else if (conjuntos[i].Contains("."))
                        { // esta mal el formato de rango 
                            alfabeto = new Dictionary<string, List<int>>();
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO EN EL RANGO, LINEA No." + cont + "\n\t" + cadena);
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
                                MessageBox.Show("ERROR DE FORMATO EN LA DEFINICION DEL CARACTER, LINEA No." + cont + "\n\t" + cadena);
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
                    MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\', LINEA No." + cont + "\n\t" + cadena);
                }                            
            }            
        }

        public bool validarIgual(string cadena) {
            cadena = quitarEspacios(cadena);
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
            if (cadena.Trim() != "")
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
                                        MessageBox.Show("ERROR DE FORMATO, LAS CLAVES ESTÁN CONCATENADAS, LINEA No." + cont + "\n\t" + cadena);                                        
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
                                    MessageBox.Show("ERROR EN EL FORMATO DEL TOKEN,LINEA No." + cont + "\n\t" + cadena);
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
                                        MessageBox.Show("EL TOKEN YA EXISTE, LINEA No." + cont + "\n\t" + cadena);                                        
                                    }
                                    //Console.WriteLine("NOMBRE VALIDO");
                                }
                            }
                        }//error de formato en el token
                        else {
                            alfabeto = new Dictionary<string, List<int>>();
                            tokens = new Dictionary<string, List<string>>();
                            error = true;
                            MessageBox.Show("ERROR DE FORMATO, NOMBRE DE TOKEN INVÁLIDO, LINEA No." + cont + "\n\t" + cadena);
                        }
                    }
                    else
                    {
                        alfabeto = new Dictionary<string, List<int>>();
                        tokens = new Dictionary<string, List<string>>();
                        error = true;
                        MessageBox.Show("ERROR DE FORMATO, NO CONTIENE LA PALARA \'TOKEN\', LINEA No." + cont + "\n\t" + cadena);
                    }

                }//SI LA CADENA NO TIENE FORMATO ADECUADO 
                else
                {
                    alfabeto = new Dictionary<string, List<int>>();
                    tokens = new Dictionary<string, List<string>>();
                    error = true;
                    MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\', LINEA No." + cont + "\n\t" + cadena);                    
                }

            }
        }

        /// <summary>
        /// Recibe la cadena del token entera desde el simbolo '=' hasta el final de la cadena. 
        /// </summary>
        /// <param name="cadena">Token completo</param>
        public List<string> obtenerToken(string cadena) {            

            //limpieza de la cadena, remover tabulaciones y reducirlas a un espacio
            cadena = cadena.Replace("\t", " ");
            while (cadena.IndexOf("  ") >= 0)
            {
                cadena = cadena.Replace("  ", " ");
            }

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
                        response = new List<string>();                       
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
                        response = new List<string>();
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
                    if (cadena[j] == '(')
                    {
                        if (cadena.IndexOf(")") == -1)
                        {
                            response = new List<string>();
                            j = cadena.Length;
                        }
                        else
                        {
                            if (aux != "")
                            {
                                response.Add(aux);
                                aux = "";
                            }
                            response.Add(cadena[j].ToString());
                        }
                    }
                    else
                    {
                        if (aux != "")
                        {
                            response.Add(aux);
                            aux = "";
                        }
                        response.Add(cadena[j].ToString());
                    }
                }
                else
                {
                    aux += cadena[j];
                }
            }

            //aux debe ser igual a "", sino hay datos extra que no deben ser leídos
            if (aux != "")
            {
                response = new List<string>();
            }


            //verificar en la lista, si hay errores en los signos
            for (int i = 0; i < response.Count; i++)
            {
                if (response[i].Equals("+") || response[i].Equals("*") || response[i].Equals("?"))
                {
                    if (i != response.Count - 1) // no verifica el ultimo porque el siguente es null
                    {
                        if (response[i + 1].Equals("+") || response[i + 1].Equals("*") || response[i + 1].Equals("?"))
                        {
                            response = new List<string>();
                        }
                    }
                }
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
                                    MessageBox.Show("LA PALABRA RESERVADA YA EXISTE, LINEA No." + cont + "\n\t" + aux);
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
                                MessageBox.Show("ERROR DE FORMATO, FALTA \', LINEA No." + cont + "\n\t" + aux);
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
                            MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\', LINEA No." + cont + "\n\t" + aux);
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
                        MessageBox.Show("ERROR DE FORMATO, NO CONTIENE EL SIMBOLO \'=\', LINEA No." + cont + "\n\t" + aux);
                    }
                }                
            }
        }


        public void leerError(string cadena) {
            if (cadena.Trim() != "")
            {
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
                            MessageBox.Show("EL ERROR YA EXISTE RESERVADA YA EXISTE, LINEA No." + cont + "\n\t" + cadena);
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
                        MessageBox.Show("ERROR DE FORMATO, LINEA No." + cont + "\n\t" + cadena);
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
                    MessageBox.Show("ERROR DE FORMATO, NO TIENE EL SINGO \'=\', LINEA No." + cont + "\n\t" + cadena);
                }
            }
        }

        private void generarDFA_Click(object sender, EventArgs e)
        {
            DFA funcionesDFA = new DFA();            
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
                if (allTokens[i].Count != 1) // si el token contiene más de un simbolo
                {
                    // si tiene más de un simbolo pero no tiene operadores 
                    if (!allTokens[i].Contains("+") && !allTokens[i].Contains("*") && !allTokens[i].Contains("?") && !allTokens[i].Contains("|"))
                    {
                        List<string> newList = new List<string>();
                        List<string> auxiliar = allTokens[i];
                        
                        for (int j = 0; j < auxiliar.Count; j++)
                        {                                                                                    
                            if (j == auxiliar.Count - 1)
                            {
                                newList.Add(auxiliar[j]);
                            }
                            else
                            {
                                newList.Add(auxiliar[j]);
                                newList.Add(".");
                            }
                        }

                        newList = funcionesDFA.transformarPostfijo(newList);
                        nodos.Add(funcionesDFA.obtenerArbol(newList)); 
                    }
                    else //contiene operadores 
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
            funcionesDFA.inicializarDiccionario(); //inicializa los valores del diccionario con listas vacías 
            funcionesDFA.calcularFollow(raiz);  //calcula los follow de cada hoja, ingresa los follow al diccionario 
            follow = funcionesDFA.getFollow();
            funcionesDFA.obtenerTerminales(raiz); // obtener simbolos terminales para poder calcular las transiciones
            funcionesDFA.simbolosTerminales = funcionesDFA.simbolosTerminales.Distinct().ToList();
            funcionesDFA.modificarTerminales(); //eliminar el elemento # de los simbolos terminales 
            //el estado principal es el first de la raíz 
            Estado inicial = new Estado(1, raiz.first);
            inicial.numero = 1;
            funcionesDFA.agregarEstado(inicial);
            //calcula las transiciones del estado inicial 
            funcionesDFA.calcularTransiciones(inicial);
            funcionesDFA.verificarNuevosEstados();
            //si hay más conjuntos determina sus transiociones
            funcionesDFA.determinarTransiciones();
            //determinar estados de aceptacion             
            funcionesDFA.determinarEstadosDeAceptacion();

            List<Estado> estados = funcionesDFA.estados;
            List<Transicion> transiciones = funcionesDFA.transiciones;
            List<string> encabezado = funcionesDFA.simbolosTerminales;
            //armar tabla de transiciones
            int filas = estados.Count;
            int columnas = encabezado.Count;
            string[,] tablaTransiciones = new string[filas, columnas];


            for (int i = 0; i < filas; i++)//fijar las filas de la matriz 
            {
                for (int j = 0; j < transiciones.Count; j++) //recorrer las transiciones existentes
                {
                    if (transiciones[j].origen == estados[i]) //si la transicion es igual al encabezado de fila
                    {
                        for (int k = 0; k < encabezado.Count; k++)
                        {
                            if (transiciones[j].simbolo == encabezado[k])
                            {
                                string conjunto = string.Join(",", transiciones[j].destino.conjunto.ToArray());
                                tablaTransiciones[i, k] = transiciones[j].destino.numero.ToString() + "={" + conjunto + "}";
                                k = encabezado.Count;
                            }                            
                        }
                    }
                }
            }

            //mostrar estado inicial 
            textBox3.Text = string.Join(",", raiz.first);
            textBox4.Text = string.Join(",", raiz.last);

            //agregar encabezado
            for (int i = 0; i < columnas + 1; i++)
            {
                if (i == 0)
                {
                    dataGridView1.Columns.Add("ESTADOS", "ESTADOS");
                }
                else
                {
                    dataGridView1.Columns.Add("simbolo" + i , encabezado[i - 1]);
                }
            }

            dataGridView1.Rows.Add(estados.Count);
            //mostrar estados
            for (int i = 0; i < filas; i++)
            {
                string conjunto = string.Join(",", estados[i].conjunto.ToArray());
                if (estados[i].aceptacion)
                {
                    dataGridView1.Rows[i].Cells[0].Value = "#" + estados[i].numero + "={" + conjunto + "}";
                    dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.LawnGreen;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[0].Value = estados[i].numero + "={" + conjunto + "}";
                }
                
            }

            //mostrar transiciones
            for (int i = 0; i < filas; i++)
            {
                for (int j = 1; j < columnas + 1; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = tablaTransiciones[i, j -1];
                }
            }


        }


        public string mostrar(List<int> lista) {
            string response = "";
            for (int i = 0; i < lista.Count; i++)
            {
                if (i == lista.Count - 1)
                {
                    response += ((char)lista[i]).ToString();
                }
                else
                {
                    response += ((char)lista[i]).ToString() + ",";
                }                
            }
            return response;
        }

        private void GUI_Load(object sender, EventArgs e)
        {

        }
    }
}
