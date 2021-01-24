using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TestePloomes
{
    class Program
    {
        static void Main(string[] args)
        {
            /************** CRIAR CLIENTE **************/
            Console.WriteLine("---------- Criar cliente ----------\n");
            JObject cliente = new JObject();

            Console.WriteLine("Você deseja criar um novo cliente? (s/n)");
            string decisao = Console.ReadLine();

            if (decisao == "s")
            {               
                Console.Write("\nDigite o nome do cliente: ");
                string nomeCliente = Console.ReadLine();
                cliente.Add("Name", nomeCliente);

                Console.WriteLine("\nO cliente é pessoa física ou jurídica? (f/j)");
                string tipoPessoa = Console.ReadLine();

                if (tipoPessoa == "f")
                    cliente.Add("TypeId", "2");
                else if (tipoPessoa == "j")
                    cliente.Add("TypeId", "1");

                AdicionarCliente(cliente);
            }
            else if (decisao == "n")
            {
                Console.WriteLine("Próxima etapa...\n");
            }

            /************** VERIFICAR ÚLTIMO CLIENTE CADASTRADO **************/
            Console.WriteLine("---------- Último cliente cadastrado ----------\n");
            JArray clientesCadastrados = RequestHandler.MakePloomesRequest($"Contacts", RestSharp.Method.GET);            
            string ultimoNomeCadastrado = clientesCadastrados.Last["Name"].ToString(); 
            Console.WriteLine($"O último cliente cadastrado é: {ultimoNomeCadastrado.ToUpper()}\n");

            /************** CRIAR NOVA NEGOCIAÇÃO **************/
            Console.WriteLine("---------- Criar Negociação ----------\n");

            JObject negocio = new JObject();

            Console.WriteLine("Digite o nome do negócio");
            string nomeNegocio = Console.ReadLine();
            negocio.Add("Title", nomeNegocio);
            Console.WriteLine("\nO negócio será vinculado ao último cliente cadastrado: " + ultimoNomeCadastrado.ToUpper() + "\n");
            negocio.Add("ContactId", clientesCadastrados.Last["Id"].ToString());

            AdicionarNegociacao(negocio);

            /************** VERIFICAR ÚLTIMO NEGÓCIO CADASTRADO **************/
            Console.WriteLine("---------- Último negócio cadastrado ----------\n");
            JArray negociosCadastrados = RequestHandler.MakePloomesRequest($"Deals", RestSharp.Method.GET);
            string ultimoNegocio = negociosCadastrados.Last["Title"].ToString();
            Console.WriteLine("Negócio cadastrado: " + ultimoNegocio.ToUpper() + "\n");

            /************** CRIAR NOVA TAREFA **************/
            Console.WriteLine("---------- Criar tarefa ----------\n");

            JObject task = new JObject();

            Console.WriteLine("Digite o nome da tarefa. Esta tarefa será vinculada ao último negócio cadastrado: " + ultimoNegocio.ToUpper() + "\n");
            string nomeTarefa = Console.ReadLine();
            task.Add("Title", nomeTarefa);
            task.Add("ContactId", clientesCadastrados.Last["Id"].ToString());
            task.Add("DealId", negociosCadastrados.Last["Id"].ToString());

            AdicionarTarefa(task);

            /************** VERIFICAR ÚLTIMA TAREFA CADASTRADA **************/
            Console.WriteLine("---------- Última tarefa cadastrada ----------\n");
            JArray tarefasCadastradas = RequestHandler.MakePloomesRequest($"Tasks", RestSharp.Method.GET);
            string ultimaTarefa = tarefasCadastradas.Last["Title"].ToString();
            Console.WriteLine("Tarefa cadastrada: " + ultimaTarefa.ToUpper() + "\n");

            /************** ALTERAR VALOR DO NEGÓCIO **************/
            Console.WriteLine("---------- Alterar Valor ----------\n");
            Console.WriteLine("Valor sendo alterado...\n");

            AtualizarValor(negociosCadastrados.Last["Id"].ToString());
            Console.WriteLine("Valor atualizado!\n");

            /************** FINALIZAR TAREFA ABERTA **************/
            Console.WriteLine("---------- Finalizar Tarefa ----------\n");
            Console.WriteLine("Tarefa sendo finalizada...\n");

            FinalizarTarefa(tarefasCadastradas.Last["Id"].ToString());

            /************** GANHAR NEGOCIAÇÃO **************/
            Console.WriteLine("---------- Ganhar Negociação ----------\n");
            Console.WriteLine("Negociação sendo ganha...\n");

            GanharNegociacao(negociosCadastrados.Last["Id"].ToString());

            /************** ESCREVER NO HISTÓRICO DO CLIENTE **************/
            Console.WriteLine("---------- Escrever no histórico ----------\n");
            Console.WriteLine("Histórico sendo escrito...\n");

            JObject interacao = new JObject();
            interacao.Add("ContactId", clientesCadastrados.Last["Id"].ToString());
            interacao.Add("Content", "Negócio Fechado!");

            AdicionarInteracao(interacao);
            Console.WriteLine("Finalizado!");
        }

        // MÉTODOS CRIADOS PARA MELHORAR A LEITURA DO CÓDIGO. ASSIM, FACILITA-SE O ENTENDIMENTO EM RELAÇÃO AOS MÉTODOS POST SENDO ADICIONADOS DURANTE A EXECUÇÃO.
        public static void AdicionarCliente(JObject cliente)
        {
            RequestHandler.MakePloomesRequest($"Contacts", RestSharp.Method.POST, cliente);
        }
       
        public static void AdicionarNegociacao(JObject negocio)
        {
            RequestHandler.MakePloomesRequest($"Deals", RestSharp.Method.POST, negocio);
        }

        public static void AdicionarTarefa(JObject tarefa)
        {
            RequestHandler.MakePloomesRequest($"Tasks", RestSharp.Method.POST, tarefa);
        }

        public static void AdicionarInteracao(JObject interacao)
        {
            RequestHandler.MakePloomesRequest($"InteractionRecords", RestSharp.Method.POST, interacao);
        }

        public static void FinalizarTarefa(string taskId)
        {
            JObject finish = new JObject
            {
                {"Finished", "true" }
            };
            RequestHandler.MakePloomesRequest($"Tasks({taskId})/Finish", RestSharp.Method.POST, finish);
        }

        public static void AtualizarValor(string dealId)
        {
            JObject update = new JObject
            {
                {"Amount", "15000.00" }
            };
            RequestHandler.MakePloomesRequest($"Deals({dealId})", RestSharp.Method.PATCH, update);
        }

        public static void GanharNegociacao(string dealId)
        {
            JObject win = new JObject
            {
                {"StatusId", "2" }
            };
            RequestHandler.MakePloomesRequest($"Deals({dealId})/Win", RestSharp.Method.POST, win);
        }
    }
}
