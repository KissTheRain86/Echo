using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Net;
using System;

public class Echo : MonoBehaviour
{
    Socket socket;
    public InputField InputField;
    public Text ReceiveText;

    //recieve
    byte[] receiveBuff = new byte[1024];
    string receiveStr = "";

    static List<Socket> checkRead = new List<Socket>();
    private void Update()
    {
        if (socket == null) return;
        checkRead.Clear();
        checkRead.Add(socket);
        //select
        Socket.Select(checkRead, null, null, 0);
        //check
       foreach(Socket s in checkRead){
            byte[] readBuff = new byte[1024];
            int count = socket.Receive(readBuff);
            string resvStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            ReceiveText.text = resvStr;
        }
    }
    public void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 8888); //同步方法
        //socket.BeginConnect(IPAddress.Parse("127.0.0.1"), 8888, ConnectCallback, socket);
    }

    public void Send()
    {
        string sendStr = InputField.text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);


        socket.Send(sendBytes);
        //socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

  

}
