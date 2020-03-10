using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class Transicion
    {
        public Estado origen = new Estado();
        public Estado destino = new Estado();
        public string simbolo { get; set; }

        public Transicion() { }

        public Transicion(Estado origen, Estado destino, string simbolo) {
            this.origen = origen;
            this.destino = destino;
            this.simbolo = simbolo;
        }
    }
}
