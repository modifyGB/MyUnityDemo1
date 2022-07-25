using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MySocket
{
    public class SendMessage 
    {
        public uint count = 0;
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public string note = "";
        public StringBuilder sb = new StringBuilder();

        public SendMessage() { }
        public SendMessage(uint count){ this.count = count; }
        public SendMessage(uint count, string note){ this.note = note; }
    }
}
