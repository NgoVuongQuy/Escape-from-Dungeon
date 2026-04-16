using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Singleton<T>
{
    protected static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
{
    if (instance != null && instance != this)
    {
        Destroy(this.gameObject);
        return;
    }
    
    instance = (T)this;

    if (!gameObject.transform.parent)
    {
        DontDestroyOnLoad(gameObject);
    }
}
}
