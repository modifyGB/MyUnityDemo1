using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Net;
using System.Text;
using System;
using Manager;

namespace MySocket
{
    public enum SocketSign { None, SaveAll, Connect, GET }

    public class Socket
    {
        private System.Net.Sockets.Socket s;
        public System.Net.Sockets.Socket S { get { return s; } }
        static private uint count = 0; 

        public Socket() { }
        public Socket(string host, int port)
        {
            s = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            S.Connect(host, port);
            Debug.Log("connect success");
        }

        public ReceiveMessage ReceiveCallback(IAsyncResult ar)
        {
            S.EndReceive(ar);
            var message = (SendMessage)ar.AsyncState;
            var rm = new ReceiveMessage(message);
            if (rm.message == null)
                return null;
            return rm;
        }

        public void Send(string message, SocketSign sign)
        {
            count++;
            string sign_ = "";
            if (sign == SocketSign.SaveAll)
                sign_ = "SAVEALL ";
            else if (sign == SocketSign.Connect)
                sign_ = "CONNECT ";
            else if (sign == SocketSign.GET)
                sign_ = "GET ";

            S.Send(Encoding.UTF8.GetBytes(count + " " + sign_ + message + '\n'));
        }

        public void Send(string message, SocketSign sign, AsyncCallback callback)
        {
            Send(message, sign);
            SendMessage m = new SendMessage(count);
            S.BeginReceive(m.buffer, 0, SendMessage.BUFFER_SIZE, SocketFlags.None, callback, m);
        }

    }
}
