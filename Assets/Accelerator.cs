using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        //Accelerator Movement
        transform.Translate(Input.acceleration.x, 0, -Input.acceleration.z);
    }
}
