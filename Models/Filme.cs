using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace santander_challenge.Models
{
    public class Filme
    {
        public int ID { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Poster { get; set; }
        public Pessoa Realizador { get; set; }
        public IEnumerable<Pessoa> Protagonistas { get; set; }
    }
}