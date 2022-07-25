using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MySocket
{
    public class ReceiveMessage
    {
        public uint count;
        public string message = null;

        public ReceiveMessage() { }
        public ReceiveMessage(SendMessage sm)
        {
            count = sm.count;
            var mList = Encoding.UTF8.GetString(sm.buffer).Split(' ');
            if (mList.Length < 2 || int.Parse(mList[0]) != count)
                return;
            message = mList[1];
        }

        public struct Serialization <T>
        {
            public T data;
        }

        public Serialization<T> ToSerialization<T>()
        {
            return JsonConvert.DeserializeObject<Serialization<T>>(message);
        }
    }
}
