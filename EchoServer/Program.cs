using System;
using System.Net;
using System.Net.Sockets;

namespace EchoServer
{
    class MainClass
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new();
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            // 建立连接socket          
            listenfd = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //bind
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEdp = new IPEndPoint(ipAddr, 8888);
            listenfd.Bind(ipEdp);
            //listen
            listenfd.Listen(0);
            Console.WriteLine("Server Start");
            //Accecpt 异步
            listenfd.BeginAccept(AcceptCallback, listenfd);
            //等待
            Console.ReadLine();
           
        }

        //1 track新的clientState
        //2 异步接收客户端数据
        //3 再次调用BeginAccept实现循环
        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("Server Accept");
                Socket listenfd = (Socket)ar.AsyncState;
                Socket clientfd = listenfd.EndAccept(ar);

                ClientState state = new ClientState();
                state.socket = clientfd;
                clients.Add(clientfd, state);

                //接收数据
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

                //循环监听accept
                listenfd.BeginAccept(AcceptCallback, listenfd);

            }catch(SocketException ex)
            {
                Console.WriteLine("Socket Accept fail" + ex.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("Server Resceive");
                ClientState state =(ClientState)ar.AsyncState;
                Socket clientfd = state.socket;
                int count = clientfd.EndReceive(ar);
                if (count == 0)
                {
                    //客户端关闭
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("Socket Close");
                    return;
                }
                string receiveStr =
                    System.Text.Encoding.UTF8.GetString(state.readBuff, 0, count);
                byte[] sendBytes =
                    System.Text.Encoding.UTF8.GetBytes("echo " + receiveStr);
                clientfd.Send(sendBytes);
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Receive fail" + ex.ToString());
            }
        }

    }

}