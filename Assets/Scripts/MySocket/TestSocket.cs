using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSocket : MonoBehaviour
{
    public MySocket.Socket socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = new MySocket.Socket("127.0.0.1", 6666);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
