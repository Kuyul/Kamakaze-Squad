using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinDynamite : MonoBehaviour
{
    public float multliplier;
    public CapsuleCollider col;

    private float TimeSince=0;

    // Update is called once per frame
    void Update()
    {
        TimeSince += Time.deltaTime * multliplier;
        transform.RotateAround(col.bounds.center,new Vector3(-0.04f,0.5f,0), multliplier);

      //  Debug.Log(transform.rotation.x);
    }

    
}
