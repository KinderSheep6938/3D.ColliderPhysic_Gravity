using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColliderLibrary.DataManager;

public class Test : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        Debug.Log(ColliderDataManager.GetColliderToWorld().Count);
    }
}
