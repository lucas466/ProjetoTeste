using Newtonsoft.Json.Linq;
using ProjetoTeste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjetoTeste.Controllers
{
    public class HomeController : Controller
    {

        //Variaveis que salvam o email a senha logados
        static String Email, Senha;

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult BitcoinPrecos()
        {
            /*Verifica se foi possivel receber os dados da API*/
            Usuario U = new Usuario();
            float Preco = U.PegaApi();

            if (Preco == 0)
            {
                ViewBag.Erro = "Infelizmente houve algum erro ao pegar a Api";
                Response.Redirect("Index");
            }
            else
            {
                ViewBag.Preco  = Preco;
            }

            return View();

        }

        public ActionResult SobreBitcoin()
        {
            return View();
        }

        public ActionResult SobreNos()
        {
            return View();
        }

        public ActionResult EntrarConta()
        {

            if (Request.HttpMethod.Equals("POST"))
            {
                /*Verifica se o usuario ja fez login*/
                try { 
                Usuario U = new Usuario();
                U.Email = Request.Form["Email"].ToString();
                U.Senha = Request.Form["Senha"].ToString();

                    if(ViewBag.Mensagem = U.EntrarUsuario().Equals("Deu certo")){
                        Email = U.Email;
                        Senha = U.Senha;
                        return RedirectToAction("VerConta", "Home");
                    }
                }
                catch 
                {
                    ViewBag.Mensagem = "Usuário e/ou senha inválido(s).";
                }

            }

            return View();
        }

        public ActionResult CriarConta()
        {

            if (Request.HttpMethod.Equals("POST"))
            {
                Usuario U = new Usuario();
                U.Email = Request.Form["Email"].ToString();
                U.Nome = Request.Form["Nome"].ToString();
                U.Telefone = Convert.ToInt32(Request.Form["Telefone"].ToString());
                U.Cartao = Convert.ToInt32(Request.Form["Cartao"].ToString());
                U.Senha = Request.Form["Senha1"].ToString();
                U.Senha2 = Request.Form["Senha2"].ToString();

                if (U.Telefone.ToString().Length >3)
                {
                    ViewBag.Userad = "Erro, use até 3 algarismo no campo telefone";

                } else if (U.Cartao.ToString().Length > 3)
                {
                    ViewBag.Userad = "Erro, use até 3 algarismo no campo Cartao";

                } else if (!U.Senha.Equals(U.Senha2))
                {
                    ViewBag.Userad = "Erro, os dois campos de senha deevem ser iguais";
                }
                else if (U.Senha.Length > 20 || U.Senha.Length > 20)
                {
                    ViewBag.Userad = "Erro, A senha pode conter no máximo 20 carateres";
                }
                else if (U.Nome.Length > 20 || U.Senha.Length > 20)
                {
                    ViewBag.Userad = "Erro, A nome pode conter no máximo 20 carateres";
                }
                else if (U.Email.Length > 20 || U.Senha.Length > 20)
                {
                    ViewBag.Userad = "Erro, O email pode conter no máximo 20 carateres";
                }
                else
                {
                    /*Verifica se o email ja existe*/
                    String resultado;

                    if (U.CriarUsuario())
                    {
                        resultado = "Conta cadastrada";
                    }
                    else
                    {
                        resultado = "Erro, email já existe";
                    }
                    ViewBag.Userad = resultado;
                }
            }

            return View();
        }

        public ActionResult VerConta()
        {
            try
            {
                /*Testa se esta vazio*/
            if (!Email.Equals("") || !Senha.Equals(""))
            {
                /*Recebe os valores do model e joga nas viewbags*/
                Usuario U = new Usuario();
                List<String> valores = new List<String>();
                valores = U.MinhaConta(Email, Senha);

                ViewBag.Nome = valores[0];
                ViewBag.Email = valores[1];
                ViewBag.Telefone = valores[2];
                ViewBag.Cartao = valores[3];
                ViewBag.Senha = valores[4];

                if (valores[5].Equals(""))
                {
                    ViewBag.BitCoin = "0";
                }
                else
                {
                    ViewBag.BitCoin = valores[5];
                }
            }

            }
            catch
            {
                Response.Redirect("/Home/Index");
            }


            return View();
        }

        public ActionResult ConfirmacaoCompra(String ID)
        {
            try
            {
                if (ID.Equals("1") || ID.Equals("5") || ID.Equals("10") || ID.Equals("20"))
                {
                    Usuario U = new Usuario();
                    int preco = Convert.ToInt32(U.PegaApi());  

                    ViewBag.Numero = ID;
                    ViewBag.Valor = preco;

                    if (Request.HttpMethod.Equals("POST"))
                    {
                        try
                        {
                            U.Email = Request.Form["Email"].ToString();
                            U.Senha = Request.Form["Senha"].ToString();
                            Email = U.Email;
                            Senha = U.Senha;
                            U.Quantidade = Convert.ToInt32(Request.Form["quantidade"].ToString());

                            if (U.Comprar(Email, Senha, U.Quantidade))
                            {
                                return RedirectToAction("VerConta", "Home");
                            }
                            else
                            {
                                ViewBag.Mensagem = "Usuário e/ou senha inválido(s).";
                            }
                        }
                        catch (Exception e)
                        {
                            ViewBag.Mensagem = e;
                        }

                    }

                }
                else
                {
                    Response.Redirect("/Home/BitcoinPrecos");
                }

            }
            catch
            {
                Response.Redirect("/Home/BitCoinPrecos");
            }
            return View();
        }
    }
}