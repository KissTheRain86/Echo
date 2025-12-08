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

            while (true)
            {
                //检查listenfd
                if (listenfd.Poll(0, SelectMode.SelectRead))
                {
                    ReadListenfd(listenfd);
                }
                //检查clientfd
                foreach (ClientState state in clients.Values)
                {
                    Socket clientfd = state.socket;
                    if (clientfd.Poll(0, SelectMode.SelectRead))
                    {
                        if (!ReadClientfd(clientfd)) break;
                    }
                }
                //防止cpu占用过高
                System.Threading.Thread.Sleep(1);
            }
        }

      
        public static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Server Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }

        public static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            //接收
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }catch(SocketException ex)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Recive SocketException" + ex.ToString());
                return false;
            }
            //客户端关闭
            if (count == 0)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }
            //收到客户端信息 广播
            string recvStr = System.Text.Encoding.UTF8.GetString(state.readBuff, 0, count);
            Console.WriteLine("Recieve:" + recvStr);
            string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
            foreach (ClientState clientState in clients.Values)
            {
                clientState.socket.Send(sendBytes);
            }
            return true;
        }

    }

}