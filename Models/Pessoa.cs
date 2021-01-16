using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace santander_challenge.Models
{
    public class Pessoa
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public IEnumerable<Filme> Filmes { get; set; }
    }
}