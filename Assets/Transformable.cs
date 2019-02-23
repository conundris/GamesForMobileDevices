using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Scale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void Rotate(float degrees)
    {
        transform.Rotate(Vector3.forward, degrees);
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }
}
