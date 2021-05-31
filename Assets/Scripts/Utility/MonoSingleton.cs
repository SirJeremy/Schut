using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonoSingleton<T> : MonoBehaviour where T : Component {
    //Made by Chris and Jeremy
    protected static T instance = null;
    protected static bool isQuitting = false;
    public static T Instance
    {
        get
        {
            if(instance == null && !isQuitting)
                FindOrCreateInstance();
            return instance;
        }
    }

    static private void FindOrCreateInstance()
    {
        T[] instanceArray = FindObjectsOfType<T>();
        if(instanceArray.Length == 0)
        {
            GameObject singleton = new GameObject();
            instance = singleton.AddComponent<T>();
            singleton.name = singleton.GetComponent<T>().ToString();
            DontDestroyOnLoad(singleton);
        }
        else if(instanceArray.Length == 1)
        {
            instance = instanceArray[0];
            DontDestroyOnLoad(instance);
        }
        else if(instanceArray.Length > 1)
        {
            //in order to reach this state, something must have gone horribly wrong
            Debug.LogError("Multiple instances of a singleton exists.");
            Debug.Break();
        }
    }

    protected virtual void OnApplicationQuit()
    {
        isQuitting = true;
    }
    protected virtual void Awake() 
    {
        T ioc = GetComponent<T>(); //instance of component
        if(instance == null) 
        {
            instance = ioc;
            DontDestroyOnLoad(instance.gameObject);
        }
        else if(instance != ioc) 
        {
            Component[] comps = GetComponents<Component>();
            if(comps.Length == 2) //transform and T, if doesnt have any other components
            {
                if(transform.childCount == 0) //if transform doesn't have any children
                    Destroy(gameObject);
                else if(transform.childCount == 1) //if it only has 1 child
                {
                    GetComponentInChildren<Transform>().parent = null;
                    Destroy(gameObject);
                }
                else
                {
                    name = "Empty " + ioc.ToString() + " (Has Children)";
                    Destroy(ioc);
                }
            }
            else //has other components
            {
                name = name + " (Removed " + ioc.ToString() + ")";
                Destroy(ioc);
            }
        }
    }
}