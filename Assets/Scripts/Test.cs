using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 r;
    public Vector3 u;
    public Vector3 f;

    // Update is called once per frame
    void Update()
    {
        r = transform.right;
        u = transform.up;
        f = transform.forward;
    }
}
