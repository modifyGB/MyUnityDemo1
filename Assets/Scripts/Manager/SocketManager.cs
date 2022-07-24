using MySocket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SocketManager : Singleton<SocketManager>
    {
        public string host;
        public int port;

        private static Socket socket;
        public static Socket Socket
        {
            get { return socket; }
            set
            {
                if (socket == null)
                    socket = value;
                else if (value == null)
                    socket = null;
            }
        }

        public override void Awake()
        {
            base.Awake();

            socket = new Socket(host, port);
        }
    }
}
