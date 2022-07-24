using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MySocket
{
    public class Message 
    {
        public uint count = 0;
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public StringBuilder sb = new StringBuilder();

        public Message() { }
        public Message(uint count)
        {
            this.count = count;
        }
    }
}
