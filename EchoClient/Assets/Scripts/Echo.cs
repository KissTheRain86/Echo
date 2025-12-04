using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;

public class Echo : MonoBehaviour
{
    Socket socket;
    public InputField InputField;
    public Text Text;

    public void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 8888);
    }

    public void Send()
    {
        string sendStr = InputField.text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        socket.Send(sendBytes);

        byte[] readBuff = new byte[1024];
        int count = socket.Receive(readBuff);
        string rescStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);

        Text.text = rescStr;
        socket.Close();
    }
}
