using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class Nodo
    {
        public string valor { get; set; }
        public bool nullable { get; set; }
        public List<string> first { get; set; }
        public List<string> last { get; set; }
        public List<string> follow { get; set; }
        public Nodo derecho { get; set; }
        public Nodo izquierdo { get; set; }

        public Nodo() { 
        }

        public Nodo(string valor, bool nullable, List<string> first, List<string> last, List<string> follow, Nodo derecho, Nodo izquierdo) {
            this.valor = valor;
            this.nullable = nullable;
            this.first = first;
            this.last = last;
            this.follow = follow;
            this.derecho = derecho;
            this.izquierdo = izquierdo;
        }


    }
}
