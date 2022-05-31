using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraphTester : MonoBehaviour
{
    void Awake()
    {
        var obj = Variables.Object(gameObject);
        // obj = GetComponent<VariableDeclarations>();
        if (obj.IsDefined("InitialDelayTime"))
        {
            obj.Set("InitialDelayTime", 2f);
        }
    }

    public void Whatever()
    {
        Debug.Log("aaaaaaaa");
    }
}
