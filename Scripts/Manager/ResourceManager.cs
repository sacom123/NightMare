using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public class ResourceManager
{
    public static UnityObject Load(string path)
    {
        //나중에 어셋 번들 로드로 변경
        return Resources.Load(path);
    }
    
    public static GameObject LoadAndInstantiate(string path)
    {
        UnityObject source = Load(path);
        if(source == null)
        {
            return null;
        }
        return GameObject.Instantiate(source) as GameObject;
    }

}
