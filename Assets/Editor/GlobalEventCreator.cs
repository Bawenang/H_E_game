using UnityEngine;
using UnityEditor;
using System.Collections;


public class GlobalEventCreator
{
    [MenuItem("Assets/Create/GlobalEvent")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<GlobalEvent>();
    }
}
