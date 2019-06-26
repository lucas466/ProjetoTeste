using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ProjetoTeste.Models
{
    public class Usuario
    {

        public String Email { get; set; }
        public String Nome { get; set; }
        public Int32 Telefone { get; set; }
        public Int32 Cartao { get; set; }
        public String Senha { get; set; }
        public String Senha2 { get; set; }
        public String Carteira { get; set; }
        public Int32 Quantidade { get; set; }

        public Usuario() { }

        public float PegaApi()
        {
            float Preco = 0;

            try {

            //Pega a api
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://s3.amazonaws.com/data-production-walltime-info/production/dynamic/meta.json?now=1517922306634.319625.92");
                JObject obj = JObject.Parse(json);
                Object valor = (Object)obj["best_offer"];

                //Transforma o resultado para  uma array
                string valorArray = "[" + valor.ToString() + "]";
                String valores;


                //Pega o valor pela array
                JArray parsedArray = JArray.Parse(valorArray);
                foreach (JObject parsedObject in parsedArray.Children<JObject>())
                {
                    foreach (JProperty NomePassado in parsedObject.Properties())
                    {
                        string NomePropriedade = NomePassado.Name;
                        if (NomePropriedade.Equals("xbt-brl"))
                        {
                            //Joga o valor para o viewbag mostrar na página
                            string propertyValue = (string)NomePassado.Value;
                            valores = propertyValue;

                            /*Calcula o valor em real*/
                            int porcentagem = Convert.ToInt32(valores.Substring(0, valores.IndexOf("/")));
                            int valorJason = Convert.ToInt32(valores.Substring(valores.IndexOf("/") + 1));

                            Preco = (100 * valorJason) / porcentagem;

                        }
                    }
                }
    
            }
            }
            catch
            {

            }

            return Preco;
    }

        public String EntrarUsuario()
        {

            String Resultado = "";

            SqlConnection connection = new SqlConnection("yourDatabase");
            connection.Open();

            try
            {

                SqlCommand commando = new SqlCommand();
                commando.Connection = connection;
                commando.CommandText = "SELECT * FROM USUARIO WHERE EMAIL = @Email AND Senha = @Senha";
                commando.Parameters.AddWithValue("@Email", Email);
                commando.Parameters.AddWithValue("@Senha", Senha);

                SqlDataReader reader = commando.ExecuteReader();

               
                if (reader.HasRows){

                    Resultado = "Deu certo";

                }
                else
                {
                    Resultado = "Usuário e/ou senha inválido(s).";
                }

            }
            catch
            {
                Resultado = "Usuário e/ou senha inválido(s).";
            } 
            finally
            {
                connection.Close();
            }

            return Resultado;

        }

        public Boolean CriarUsuario()
        {
            Boolean Resultado = false;
            SqlConnection connection = new SqlConnection("yourDatabase");
            connection.Open();
            try
            {
                SqlCommand commando = new SqlCommand();
                commando.Connection = connection;
                commando.CommandText = "INSERT INTO USUARIO (Email, Nome, Telefone, Cartao, Senha) VALUES (@Email, @Nome, @Telefone, @Cartao, @Senha)";
                commando.Parameters.AddWithValue("@Email", Email);
                commando.Parameters.AddWithValue("@Nome", Nome);
                commando.Parameters.AddWithValue("@Telefone", Telefone);
                commando.Parameters.AddWithValue("@Cartao", Cartao);
                commando.Parameters.AddWithValue("@Senha", Senha);

                if (commando.ExecuteNonQuery() > 0)
                {
                    Resultado = true;
                }

            }
            catch 
            {
                Resultado = false;
            }

            finally
            {
                connection.Close();
            }
            return Resultado;

        }

        public List<String> MinhaConta(String Email, String Senha)
        {
            List<String> valores = new List<String>();

            SqlConnection connection = new SqlConnection("yourDatabase");
            connection.Open();
            try
            {
                SqlCommand commando = new SqlCommand();
                commando.Connection = connection;
                commando.CommandText = "SELECT * FROM USUARIO WHERE Email = @Email AND Senha = @Senha";
                commando.Parameters.AddWithValue("@Email", Email);
                commando.Parameters.AddWithValue("@Senha", Senha);

                SqlDataReader Leitor = commando.ExecuteReader();

                while (Leitor.Read())
                {
                    Usuario U = new Usuario();
                    valores.Add(U.Nome = Leitor["Nome"].ToString());
                    valores.Add(U.Email = Leitor["Email"].ToString());
                    U.Telefone = Convert.ToInt32(Leitor["Telefone"]);
                    valores.Add(Convert.ToString(U.Telefone));
                    U.Cartao = Convert.ToInt32(Leitor["Cartao"]);
                    valores.Add(Convert.ToString(U.Cartao));
                    valores.Add(U.Senha = Leitor["Senha"].ToString());
                    valores.Add(U.Carteira = Leitor["Carteira"].ToString());

                }

                return valores;
            }
            catch
            {

                return null;
            }
            finally
            {
                connection.Close();
            };

        }

        public Boolean Comprar(String Email, String Senha, Int32 Valor)
        {
            Boolean Resultado = false;

            if (EntrarUsuario().Equals("Deu certo"))
            {
                List<String> valor = new List<String>();
                valor = MinhaConta(Email, Senha);

                int valorAtual, ValorResutado;

                if (valor[5].Equals(""))
                {
                    valorAtual = 0;
                }
                else
                {
                    valorAtual = Convert.ToInt32(valor[5]);
                }

                ValorResutado = valorAtual + Valor;

                SqlConnection connection = new SqlConnection("yourDatabase");
                connection.Open();

                SqlCommand commando = new SqlCommand();
                commando.Connection = connection;
                commando.CommandText = "UPDATE USUARIO SET Carteira = @Carteira WHERE Email = @Email";
                commando.Parameters.AddWithValue("@Carteira", ValorResutado);
                commando.Parameters.AddWithValue("@Email", Email);

                if (commando.ExecuteNonQuery() > 0)
                {
                    Resultado = true;
                }

               
            }
            return Resultado;
        }
         
        
    }
}