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


    private void Update()
    {
        if (socket == null) return;
        if (socket.Poll(0, SelectMode.SelectRead))
        {
            //如果有可读数据 读取
            byte[] readBuff = new byte[1024];
            int count = socket.Receive(readBuff);
            string recvStr = 
                System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            receiveStr = recvStr + "\n" + receiveStr;
            ReceiveText.text = receiveStr;
        }
        
        //注意socket的回调是在后台线程执行 不是在主线程执行的
        //因此只能这样赋值
        //ReceiveText.text = receiveStr;
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
