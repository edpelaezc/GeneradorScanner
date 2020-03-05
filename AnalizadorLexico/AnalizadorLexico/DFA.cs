﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class DFA
    {
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
    }
}