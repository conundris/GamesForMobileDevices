using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroTest : MonoBehaviour
{
    private bool gyroEnable;
    private UnityEngine.Gyroscope gyro;
 
    // Use this for initialization
    void Start () {
        // enable the gyroscope
        gyroEnable = EnableGyro();
		
    }
 
    private bool EnableGyro()
    {
        // test the gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
 
            return true;
        }
        return false;
    }
 
    // Update is called once per frame
    void Update () {
        if (gyroEnable)
        {
            // show info from gyroscope 
            Debug.Log("attitude "+ gyro.attitude);
            Debug.Log("gravity " + gyro.gravity);
            Debug.Log("userAcceleration " + gyro.userAcceleration);
        }
    }
}
