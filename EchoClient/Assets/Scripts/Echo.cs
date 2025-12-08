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
        //注意socket的回调是在后台线程执行 不是在主线程执行的
        //因此只能这样赋值
        ReceiveText.text = receiveStr;
    }
    public void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //socket.Connect("127.0.0.1", 8888); 同步方法
        socket.BeginConnect(IPAddress.Parse("127.0.0.1"), 8888, ConnectCallback, socket);
    }

    public void Send()
    {
        string sendStr = InputField.text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        //socket.Send(sendBytes);
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            socket.BeginReceive(receiveBuff, 0, 1024, 0, ReceiveCallback,socket);
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
        }
    }

    //注意socket的回调是在后台线程执行 不是在主线程执行的
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            Debug.Log("Socket recieve succ" + count);

            string s = System.Text.Encoding.UTF8.GetString(receiveBuff, 0, count);
            receiveStr = s + "\n" +receiveStr;

            //ReceiveText.text = receiveStr;//注意不能这样 后台线程合主线程混用了
            socket.BeginReceive(receiveBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket )ar.AsyncState;
            int count = socket.EndSend(ar);
            Debug.Log("Socket send succ" + count);
        }catch(SocketException ex)
        {
            Debug.Log("Socket send fail" + ex.ToString());
        }
    }

}
