using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatCliente
{
    class Program
    {
        private static string id = "Default";
        private static Socket sender;
        private static bool monitorarMsgRecebidas;

        private static void AlterarID()
        {
            string confirma;
            string newID;

            Console.WriteLine("--[ ID ]-----------");
            do
            {
                Console.Write("informe seu ID: ");
                newID = LerTeclado().Replace("[", "").Replace("]", "");

                Console.Write("confirma [ " + newID + " ] ? <s/n>: ");
                confirma = LerTeclado();

                Console.WriteLine("\n--[ ID ]-----------");
            } while (!confirma.ToLower().Equals("s"));

            id = newID;
        }

        private static void EnviarMensagem(String msg = "")
        {
            string msg_ok = "[ " + id + " ]: " + msg + "*";

            byte[] messageSent = Encoding.ASCII.GetBytes(msg_ok);
            _ = sender.Send(messageSent);
        }

        private static void ReceberMensagem()
        {
            while (monitorarMsgRecebidas)
            {
                try
                {
                    byte[] receber = new byte[1024];
                    string retorno;
                    do
                    {
                        int dadosRecebidos = sender.Receive(receber);
                        retorno = Encoding.ASCII.GetString(receber, 0, dadosRecebidos);
                    } while (retorno.LastIndexOf("*") != (retorno.Length - 1));
                    Console.WriteLine(MsgRecebida(retorno));
                    Thread.Sleep(50);
                }
                catch (Exception)
                {
                    monitorarMsgRecebidas = false;
                }
            }
        }

        private static string MsgRecebida(string msg)
        {
            string novaMsg;
            if (msg.ToLower().Contains("/sair*"))
            {
                string id = msg.Substring(0, msg.LastIndexOf("]") + 1);
                novaMsg = "<<<< " + id + ": saiu...";
            }
            else
            {
                novaMsg = ">>>> " + msg.Substring(0, msg.Length - 1);
            }

            return novaMsg;
        }

        private static string LerTeclado()
        {
            return Console.ReadLine();
        }

        private static void Encerrar()
        {
            monitorarMsgRecebidas = false;
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        static void Main(string[] args)
        {
            /* DEFININDO IP E PORTAS */
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            // coletar ip atual - sempre a primeira posição do "addresslist" é o IP atual
            IPAddress ipAddr = ipHost.AddressList[0];
            // esse endpoint é o que receberá a mansagem enviada por esse cliente;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
            // socket que irá enviar requisições ao servidor
            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            AlterarID();
            Console.Clear();

            String msg = "";
            try
            {
                sender.Connect(localEndPoint);
                monitorarMsgRecebidas = true;
                Console.WriteLine("Conectado como [ " + id + " ] ---- para sair digite </sair>");
                EnviarMensagem("Conectou..");

                // monitorar recebimento de mensagens
                Thread recebeMsg = new Thread(ReceberMensagem);
                recebeMsg.Start();

                do
                {
                    msg = LerTeclado();
                    EnviarMensagem(msg);

                } while (!msg.ToLower().Contains("/sair"));

                Encerrar();
            }
            catch (Exception)
            {
                Console.WriteLine("[ system ]: sem conexão com o servidor..");
                Encerrar();
            }
        }
    }
}

