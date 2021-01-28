using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static bool isQuitting;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Make sure there isnt another instance of the same type in memory
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            // If null, make this instance the Singleton instance
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy current instance because it is a duplicate
            Destroy(gameObject);
        }
    }
}
