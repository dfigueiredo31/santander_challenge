using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using santander_challenge.Models;

namespace santander_challenge.Controllers
{
    public class HomeController : Controller
    {
        static readonly Uri enderecoAPI = new Uri("https://api.themoviedb.org");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Pesquisa()
        {
            PesquisaViewModel resultados = TempData["Pesquisa"] as PesquisaViewModel;
            return View(resultados);
        }

        public ActionResult GetInfo(Pessoa pesquisa)
        {
            using(var cliente = new HttpClient())
            {
                cliente.BaseAddress = enderecoAPI;
                var resposta = cliente.GetAsync("/3/search/person?api_key=3d187e4ded765515e07940389b933fa8&language=pt-BR&query=" + pesquisa.Nome);
                resposta.Wait();
                if (resposta.Result.IsSuccessStatusCode)
                {
                    var lerResposta = resposta.Result.Content.ReadAsStringAsync();
                    lerResposta.Wait();

                    JObject respostaJson = JObject.Parse(lerResposta.Result);
                    JArray conhecidoPor = (JArray)respostaJson.SelectToken("results[0]").SelectToken("known_for");
                    JToken infoPessoal = respostaJson.SelectToken("results[0]");

                    Pessoa pessoaPesquisada = new Pessoa
                    {
                        ID = Int32.Parse(infoPessoal.SelectToken("id").ToString()),
                        Nome = infoPessoal.SelectToken("name").ToString(),
                        Funcao = infoPessoal.SelectToken("known_for_department").ToString(),
                        Foto = "https://image.tmdb.org/t/p/original" + infoPessoal.SelectToken("profile_path")
                    };

                    List<Filme> filmes = new List<Filme>();

                    foreach (JToken filme in conhecidoPor)
                    {
                        //pedir o cast deste filme
                        var cast = cliente.GetAsync($"/3/movie/{filme.SelectToken("id")}/credits?api_key=3d187e4ded765515e07940389b933fa8");
                        var castResposta = cast.Result.Content.ReadAsStringAsync();
                        castResposta.Wait();

                        JObject castRespostaJson = JObject.Parse(castResposta.Result);
                        JArray castArray = (JArray)castRespostaJson.SelectToken("cast");


                        List<Pessoa> atores = new List<Pessoa>();
                        List<Pessoa> equipaTecnica = new List<Pessoa>();
                        
                        foreach(JToken membroCast in castArray)
                        {
                            if(membroCast.SelectToken("known_for_department").ToString() == "Acting")
                            {
                                atores.Add(new Pessoa
                                {
                                    ID = Int32.Parse(membroCast.SelectToken("id").ToString()),
                                    Nome = membroCast.SelectToken("name").ToString()
                                });
                            }else if (membroCast.SelectToken("known_for_department").ToString() == "Directing")
                            {
                                equipaTecnica.Add(new Pessoa
                                {
                                    ID = Int32.Parse(membroCast.SelectToken("id").ToString()),
                                    Nome = membroCast.SelectToken("name").ToString()
                                });
                            }
                        }

                        filmes.Add(new Filme
                        {
                            ID = Int32.Parse(filme.SelectToken("id").ToString()),
                            Titulo = filme.SelectToken("original_title").ToString(),
                            Descricao = filme.SelectToken("overview").ToString(),
                            DataLancamento = DateTime.Parse(filme.SelectToken("release_date").ToString()),
                            Poster = "https://image.tmdb.org/t/p/original" + filme.SelectToken("poster_path"),
                            Protagonistas = atores,
                            Realizadores = equipaTecnica
                        }); ;
                    }

                    PesquisaViewModel resultados = new PesquisaViewModel() 
                    {
                        Pessoa = pessoaPesquisada, 
                        ListaFilmes = filmes
                    };

                    TempData["Pesquisa"] = resultados;
                    return RedirectToAction("Pesquisa");
                }
                return RedirectToAction("Index");   //TODO: redirecionar para uma pagina de erro
            }
        }
    }
}