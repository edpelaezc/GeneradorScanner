using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class Nodo
    {
        public List<int> first = new List<int>();
        public List<int> last = new List<int>();
        public string valor { get; set; }
        public bool nullable { get; set; }
        public int num { get; set; }
        public Nodo derecho { get; set; }
        public Nodo izquierdo { get; set; }

        public Nodo() { 
        }

        public Nodo(string valor, bool nullable, List<int> first, List<int> last, Nodo derecho, Nodo izquierdo) {
            this.valor = valor;
            this.nullable = nullable;
            this.first = first;
            this.last = last;           
            this.derecho = derecho;
            this.izquierdo = izquierdo;
        }


    }
}
