using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Net;
using System.Text;
using System;

namespace MySocket
{
    public enum SocketSign { SaveAll, Connect }

    public class Socket
    {
        private System.Net.Sockets.Socket s;
        public System.Net.Sockets.Socket S { get { return s; } }
        private byte[] buffer = new byte[1024];

        public Socket() { }
        public Socket(string host, int port)
        {
            s = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            S.Connect(host, port);
            Debug.Log("connect success");
            S.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            S.EndReceive(ar);
            Debug.Log(Encoding.UTF8.GetString(buffer));
        }

        public void Send(string message, SocketSign sign)
        {
            string sign_ = "";
            if (sign == SocketSign.SaveAll)
                sign_ = "SAVEALL ";
            if (sign == SocketSign.Connect)
                sign_ = "CONNECT ";
            S.Send(Encoding.UTF8.GetBytes(sign_ + message + '\n'));
        }
    }
}
