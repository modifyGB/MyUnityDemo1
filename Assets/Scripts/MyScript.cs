using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    //�����Լ�
    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }
}
