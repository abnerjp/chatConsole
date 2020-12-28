using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServidor
{
    class LidarComCliente
    {
        // lista de clientes conectados ao servidor
        private static List<Socket> clientes = new List<Socket>();
        
        //atributos do objeto
        private Socket cliente;
        private bool escutarCliente = true;

        public LidarComCliente(Socket cliente)
        {
            this.cliente = cliente;
            clientes.Add(cliente);

            Thread clienteThread = new Thread(AtenderCliente);
            clienteThread.Start();
        }

        private void AtenderCliente()
        {
            try
            {
                while (escutarCliente)
                {
                    byte[] bytes = new byte[1024];
                    string msg = null;
                    do
                    {
                        int dadosRecebidos = this.cliente.Receive(bytes);
                        msg += Encoding.ASCII.GetString(bytes, 0, dadosRecebidos);
                    } while (msg.LastIndexOf("*") != (msg.Length - 1));

                    Console.WriteLine(MsgRecebida(msg));

                    // enviar msg deste cliente para todos os outros
                    byte[] resposta = Encoding.ASCII.GetBytes(msg);
                    foreach (Socket cli in clientes)
                    {
                        if (!this.cliente.Equals(cli))
                            cli.Send(resposta);
                    }
                    Thread.Sleep(50);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Cliente desconectou-se inesperadamente");
                Encerrar();
                this.cliente.Shutdown(SocketShutdown.Both);
                this.cliente.Close();
            }
        }

        private void Encerrar()
        {
            clientes.Remove(this.cliente);
            this.escutarCliente = false;
        }

        private string MsgRecebida(string msg)
        {
            string novaMsg;
            if (msg.ToLower().Contains("/sair*"))
            {
                String id = msg.Substring(0, msg.LastIndexOf("]") + 1);
                novaMsg = "<<<< " + id + ": saiu...";
                Encerrar();
            }
            else
            {
                novaMsg = ">>>> " + msg[0..^1];
                //novaMsg = ">>>> " + msg.Substring(0, msg.Length - 1);
            }
            return novaMsg;
        }
    }
}
