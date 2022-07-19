using UnityEngine;

//单例类
public abstract class Singleton<T> : MyScript where T : MyScript
{
    private static T instance;
    [SerializeField] private bool destroyOnLoad = false;

    public static T I
    {
        get { return instance; }
        set
        {
            if (instance == null)
            {
                instance = value;
            }
            else if (instance != value)
            {
                Destroy(value.gameObject);
            }
        }
    }

    public virtual void Awake()
    {
        I = this as T;
        if (!destroyOnLoad)
        {
            DontDestroyOnLoad(this);
        }

    }
}
