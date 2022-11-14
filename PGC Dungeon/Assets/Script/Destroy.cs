using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public bool fromStart = false;
    public float time = 3;

    void Start()
    {
        if (fromStart) { Destroy(this.gameObject, time); }
    }


    public void CallDestroy()
    {
            Destroy(this.gameObject);
    }
}
