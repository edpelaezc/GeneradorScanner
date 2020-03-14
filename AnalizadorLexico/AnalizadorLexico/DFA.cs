using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class DFA
    {
        Dictionary<int, List<int>> response = new Dictionary<int, List<int>>();
        List<Nodo> terminales = new List<Nodo>();
        public List<string> simbolosTerminales = new List<string>();
        public List<Estado> estados = new List<Estado>();
        public List<Transicion> transiciones = new List<Transicion>();
        StreamWriter outputFile = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\output.txt");

        public List<string> transformarPostfijo(List<string> infijo)
        {
            Stack<string> operadores = new Stack<string>();
            List<string> response = new List<string>();

            //recorrer token y transformarlo a postfijo
            for (int i = 0; i < infijo.Count; i++)
            {
                if (infijo[i] == "(")
                {
                    operadores.Push(infijo[i]);
                }
                else if (infijo[i] == ")")
                {
                    while (operadores.Peek() != "(")
                    {
                        response.Add(operadores.Pop());
                    }
                    operadores.Pop();
                }
                else if (infijo[i] == "*" || infijo[i] == "+" || infijo[i] == "?" || infijo[i] == "|" || infijo[i] == ".")
                {
                    if (operadores.Count != 0)
                    {
                        if (precedencia(operadores.Peek()) >= precedencia(infijo[i]))
                        {
                            response.Add(operadores.Pop());
                        }
                    }

                    operadores.Push(infijo[i]);
                }
                else // es un operador
                {
                    response.Add(infijo[i]); //agregar al final de la lista de salida
                }
            }

            if (operadores.Count != 0)
            {
                int count = operadores.Count;
                for (int i = 0; i < count; i++)
                {
                    response.Add(operadores.Pop());
                }
            }
            return response;
        }

        private static int precedencia(string op)
        {
            int orden = 0;
            if (op.Equals("|")) orden = 3;
            if (op.Equals(".")) orden = 4;
            if (op.Equals("*") || op.Equals("+") || op.Equals("?")) orden = 5;
            if (op.Equals(")")) orden = 2;
            if (op.Equals("(")) orden = 1;
            return orden;
        }

        public Nodo obtenerArbol(List<string> token) {
            Nodo salida = new Nodo();
            Stack<Nodo> operandos = new Stack<Nodo>();
            List<Nodo> postfijo = new List<Nodo>();

            for (int i = 0; i < token.Count; i++)
            {
                Nodo aux = new Nodo();
                aux.valor = token[i];
                postfijo.Add(aux);
            }

            for (int i = 0; i < token.Count; i++)
            {
                if (postfijo[i].valor == "|" || postfijo[i].valor == ".")
                {
                    postfijo[i].derecho = operandos.Pop();
                    postfijo[i].izquierdo = operandos.Pop();                   
                    operandos.Push(postfijo[i]);
                }
                else if (postfijo[i].valor == "+" || postfijo[i].valor == "*" || postfijo[i].valor == "?")
                {
                    postfijo[i].izquierdo = operandos.Pop();
                    operandos.Push(postfijo[i]);
                }
                else
                {                    
                    operandos.Push(postfijo[i]); // es un operando 
                }
            }

            salida = operandos.Pop();


            return salida; 
        }


        public Nodo obtenerArbolCompleto(List<Nodo> nodos) {
            Nodo salida = new Nodo();
            Stack<Nodo> operadores = new Stack<Nodo>();
            List<Nodo> response = new List<Nodo>();

            //procesar en postfijo todos los nodos 
            for (int i = 0; i < nodos.Count; i++)
            {
                if ((nodos[i].valor == "|" && nodos[i].derecho == null && nodos[i].izquierdo == null) || (nodos[i].valor == "." && nodos[i].derecho == null && nodos[i].izquierdo == null))
                {
                    if (operadores.Count != 0)
                    {
                        response.Add(operadores.Pop());
                    }

                    operadores.Push(nodos[i]);
                }
                else // es un operador
                {
                    response.Add(nodos[i]); //agregar al final de la lista de salida
                }
            }

            //verificar si aún existen operadores en la pila
            if (operadores.Count != 0)
            {
                int count = operadores.Count;
                for (int i = 0; i < count; i++)
                {
                    response.Add(operadores.Pop());
                }
            }
           
            //operar los nodos en postfijo
            Stack<Nodo> operandos = new Stack<Nodo>();
            while (response.Count != 0)
            {
                Nodo aux = response[0];
                response.RemoveAt(0);

                if ((aux.valor == "|" && aux.derecho == null && aux.izquierdo == null) || (aux.valor == "." && aux.derecho == null && aux.izquierdo == null))
                {
                    aux.derecho = operandos.Pop();
                    aux.izquierdo = operandos.Pop();
                    response.Insert(0, aux);
                }
                else
                {
                    operandos.Push(aux); // es un operando 
                }
            }

            salida = operandos.Pop();


            return salida;

        }

        int contador = 0;
        public void contarNodosHoja(Nodo root) {
            if (root != null)
            {
                contarNodosHoja(root.izquierdo);
                contarNodosHoja(root.derecho);

                if (root.derecho == null && root.izquierdo == null) { 
                    contador++;
                    root.num = contador;
                    Console.WriteLine(root.valor + ", " + root.num);
                }
            }
        }


        public void calcularNulabilidad(Nodo root) {
            if (root != null)
            {
                if (root.valor == "|" && root.derecho != null && root.izquierdo != null)
                {
                    if (root.derecho.nullable == true || root.izquierdo.nullable == true)
                        root.nullable = true;
                    else
                        root.nullable = false;
                }
                else if (root.valor == "." && root.derecho != null && root.izquierdo != null)
                {
                    if (root.derecho.nullable == true && root.izquierdo.nullable == true)
                        root.nullable = true;
                    else
                        root.nullable = false;
                }
                else if (root.valor == "*" && root.izquierdo != null)
                {
                    root.nullable = true;
                }
                else if (root.valor == "+" && root.izquierdo != null)
                {
                    root.nullable = false;
                }
                else if (root.valor == "?" && root.izquierdo != null)
                {
                    root.nullable = false;
                }
                else
                {
                    root.nullable = false;
                    //not null -> false 
                    //null -> true
                }

                calcularNulabilidad(root.izquierdo);
                calcularNulabilidad(root.derecho);                
            }            
        }

        public void first(Nodo root) {
            if (root != null)
            {
                first(root.izquierdo);
                first(root.derecho);

                if (root.valor == "|" && root.derecho != null && root.izquierdo != null)
                {
                    root.first.AddRange(root.izquierdo.first);
                    root.first.AddRange(root.derecho.first);
                    root.first.Sort();
                }
                else if (root.valor == "." && root.derecho != null && root.izquierdo != null)
                {
                    if (root.izquierdo.nullable == true)
                    {
                        root.first.AddRange(root.izquierdo.first);
                        root.first.AddRange(root.derecho.first);
                        root.first.Sort();
                    }
                    else
                    {
                        root.first.AddRange(root.izquierdo.first);                        
                    }
                }
                else if ((root.valor == "*" || root.valor == "+" || root.valor == "?") && (root.izquierdo != null) )
                {//se determina que es un operador unario
                    root.first.AddRange(root.izquierdo.first);
                }
                else 
                {
                    //es un nodo con simbolo terminal, nodo hoja
                    root.first.Add(root.num);
                }
            }
        }

        public void last(Nodo root) {
            if (root != null)
            {
                last(root.izquierdo);
                last(root.derecho);

                if (root.valor == "|" && root.derecho != null && root.izquierdo != null)
                {
                    root.last.AddRange(root.izquierdo.last);
                    root.last.AddRange(root.derecho.last);
                    root.last.Sort();
                }
                else if (root.valor == "." && root.derecho != null && root.izquierdo != null)
                {
                    if (root.derecho.nullable == true)
                    {
                        root.last.AddRange(root.izquierdo.last);
                        root.last.AddRange(root.derecho.last);
                        root.last.Sort();
                    }
                    else
                    {
                        root.last.AddRange(root.derecho.last);
                    }
                }
                else if ((root.valor == "*" || root.valor == "+" || root.valor == "?") && (root.izquierdo != null))
                {
                    root.last.AddRange(root.izquierdo.last);
                }
                else 
                {
                    //es un nodo con simbolo terminal, nodo hoja
                    root.last.Add(root.num);
                }
            }
        }

        public void inicializarDiccionario() {
            for (int i = 1; i <= contador; i++)
            {
                response.Add(i, new List<int> { });
            }
        }

        public void calcularFollow(Nodo root) {
            if (root != null)
            {
                calcularFollow(root.izquierdo);
                calcularFollow(root.derecho);
                outputFile.WriteLine("NODO: " + root.valor + "\nFIRST: " + string.Join(",", root.first.ToArray()) + "\tLAST: " + string.Join(",", root.last.ToArray()) + "\n");

                if (root.valor == "." && root.derecho != null && root.izquierdo != null)
                {
                    for (int i = 0; i < root.izquierdo.last.Count; i++)
                    {
                        for (int j = 0; j < response.Count; j++)
                        {
                            if (response.Keys.ToList()[j] == root.izquierdo.last[i])
                            {
                                response.Values.ToList()[j].AddRange(root.derecho.first);
                                response.Values.ToList()[j].Sort();
                                response.Values.ToList()[j] = response.Values.ToList()[j].Distinct().ToList();
                            }
                        }
                    }
                }
                else if ((root.valor == "*" || root.valor == "+" ) && root.izquierdo != null)
                {
                    for (int i = 0; i < root.izquierdo.last.Count; i++)
                    {
                        for (int j = 0; j < response.Count; j++)
                        {
                            if (response.Keys.ToList()[j] == root.izquierdo.last[i])
                            {
                                response.Values.ToList()[j].AddRange(root.izquierdo.first);
                                response.Values.ToList()[j].Sort();
                                response.Values.ToList()[j] = response.Values.ToList()[j].Distinct().ToList();
                            }
                        }
                    }
                }
            }                                
        }

        public Dictionary<int, List<int>> getFollow() {
            return response;
        }


        public void obtenerTerminales(Nodo root) {
            if (root != null)
            {
                obtenerTerminales(root.izquierdo);
                obtenerTerminales(root.derecho);               

                if (root.derecho == null && root.izquierdo == null)
                {
                    terminales.Add(root);
                    simbolosTerminales.Add(root.valor);
                }
            }
        }

        public void agregarEstado(Estado estado) {
            estados.Add(estado);
        }

        public void modificarTerminales() {
            simbolosTerminales.Remove("#");
        }

        public void calcularTransiciones(Estado estado) {
            //recorrer símbolos terminales
            for (int i = 0; i < simbolosTerminales.Count; i++)
            {
                Transicion auxTransicion = new Transicion();
                
                for (int j = 0; j < estado.conjunto.Count; j++)
                {
                    if (terminales[estado.conjunto[j] - 1].valor == simbolosTerminales[i])
                    {                        
                        auxTransicion.destino.conjunto.AddRange(response[estado.conjunto[j]]);
                        auxTransicion.destino.conjunto.Sort();
                        auxTransicion.destino.conjunto = auxTransicion.destino.conjunto.Distinct().ToList();
                    }
                }

                if (auxTransicion.destino.conjunto.Count != 0)
                {
                    auxTransicion.simbolo = simbolosTerminales[i];
                    auxTransicion.origen = estado;
                    transiciones.Add(auxTransicion);
                }
            }
        }

        public void verificarNuevosEstados() {
            bool exists = false;
            int posicion = 0;
            //ver las transiciones del estado inicial
            for (int i = 0; i < transiciones.Count; i++)
            {
                exists = false;
                for (int j = 0; j < estados.Count; j++)
                {
                    if (compararConjuntos(estados[j].conjunto, transiciones[i].destino.conjunto) )
                    {
                        exists = true;
                        posicion = estados[j].numero;
                    }
                }

                if (exists)
                {
                    transiciones[i].destino.numero = posicion;
                }
                else //si no existía el conjunto
                {
                    int nuevoEstado = estados[estados.Count - 1].numero + 1;
                    transiciones[i].destino.numero = nuevoEstado;
                    estados.Add(transiciones[i].destino);
                }

            }
        }


        public bool compararConjuntos(List<int> conjunto1, List<int> conjunto2) {
            return conjunto1.SequenceEqual(conjunto2);           
        }

        public void determinarTransiciones() {
            int inicial = estados.Count; // numero de estados inicial 
            int indice = 1;

            do
            {
                inicial = estados.Count;
                if (estados.Count != 1) // si hay más de un conjunto
                {
                    for (int i = indice; i < estados.Count; i++)
                    {
                        calcularTransiciones(estados[i]);
                    }

                    verificarNuevosEstados();
                    indice = inicial;
                }
            } while (estados.Count != inicial);

        }

        public void determinarEstadosDeAceptacion() {
            for (int i = 0; i < estados.Count; i++)
            {
                if (estados[i].conjunto.Contains(contador)) //contador queda con el numero de la ultima hoja #
                {
                    estados[i].aceptacion = true;
                }
                else
                {
                    estados[i].aceptacion = false;
                }
            }
        }

        public void cerrarWriter() {
            outputFile.Close();
        }

    }
}
