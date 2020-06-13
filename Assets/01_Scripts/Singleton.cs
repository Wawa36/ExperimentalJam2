using System.Reflection;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public static T Instance
    {
        get 
        {
            if (instance)
            {
                return instance;
            }
            else
            {
                var t = GameObject.FindObjectOfType<T>();

                if (t)
                    return t;
                else
                    return new GameObject("Singleton: " + typeof(T).ToString()).AddComponent<T>();
            }
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
