using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace santander_challenge.Models
{
    public class PesquisaViewModel
    {
        public Pessoa Pessoa { get; set; }
        public IEnumerable<Filme> ListaFilmes { get; set; }
    }
}