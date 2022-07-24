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
    public enum SocketSign { None, SaveAll, Connect }

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

        public string ReceiveCallback(IAsyncResult ar)
        {
            S.EndReceive(ar);
            var message = (Message)ar.AsyncState;
            var mList = Encoding.UTF8.GetString(message.buffer).Split(' ');
            if (mList.Length < 2 || int.Parse(mList[0]) != message.count)
                return null;
            return mList[1];
        }

        public void Send(string message, SocketSign sign)
        {
            count++;
            string sign_ = "";
            if (sign == SocketSign.SaveAll)
                sign_ = "SAVEALL ";
            if (sign == SocketSign.Connect)
                sign_ = "CONNECT ";

            S.Send(Encoding.UTF8.GetBytes(count + " " + sign_ + message + '\n'));
        }

        public void Send(string message, SocketSign sign, AsyncCallback callback)
        {
            Send(message, sign);
            Message m = new Message(count);
            S.BeginReceive(m.buffer, 0, Message.BUFFER_SIZE, SocketFlags.None, callback, m);
        }

    }
}
