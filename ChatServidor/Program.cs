using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServidor
{
    class Program
    {
        static void Main(string[] args)
        {
            // status do servidor
            bool ativo = true;

            // DEFININDO IP E PORTAS
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            // coletar ip atual - sempre a primeira posição do "addresslist" é o IP atual
            IPAddress ipAddr = ipHost.AddressList[0];
            // esse endpoint estará aguardando requisição na porta 11111
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
            // definição de como o socket irá funcionar (EM QUAL PORTA IRÁ ATUAR, TIPO DE PROTOCOLO)
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
           
            try
            {
                // colocar socket para escutar a porta definida acima;
                listener.Bind(localEndPoint);

                //socket definido para atender 10 requisições simultaneamente
                listener.Listen(10);

                Console.WriteLine("aguardando conexões do cliente ...");
                while (ativo)
                {
                    // socket para atender requisições do cliente
                    Socket cliente = listener.Accept();

                    // aqui inicia thread
                    LidarComCliente lidarCliente = new LidarComCliente(cliente);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao iniciar o servidor\n" + e.ToString());
            }
        }
    }
}

