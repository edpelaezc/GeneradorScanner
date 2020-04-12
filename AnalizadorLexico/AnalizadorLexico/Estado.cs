using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class Estado
    {
        public List<int> conjunto = new List<int>();
        public int numero { get; set; }

        //true ->estado de aceptación 
        public bool aceptacion { get; set; }

        public List<KeyValuePair<List<string>, int>> token = new List<KeyValuePair<List<string>, int>>();

        public Estado() { }

        public Estado(int numero, List<int> conjunto) {
            this.conjunto = conjunto;
            this.numero = numero;
        }
    }
}
