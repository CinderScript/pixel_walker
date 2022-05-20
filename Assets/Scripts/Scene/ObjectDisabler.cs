using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
    [Header("Disable On/Off")]
    public bool DisableObjects;
    
    [Header("Objects")]
    public List<GameObject> objects;
    
    void Awake()
    {
        if (DisableObjects)
        {
            foreach (var obj in objects)
            {
                obj.SetActive(false);
            }
        }
    }
}
