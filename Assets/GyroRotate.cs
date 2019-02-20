using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroRotate : MonoBehaviour
{
    private bool gyroEnable;
    private UnityEngine.Gyroscope gyro;
    
    // Start is called before the first frame update
    void Start()
    {
        gyro = Input.gyro;
        if(!gyro.enabled)
        {
            gyro.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation  = gyro.attitude;
    }
}
