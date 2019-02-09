using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float speed = 10f;
	
	void Update () {

        //Forwards
        if (Input.GetKey("w"))
        {
            Move(transform.forward, speed);
        }

        //Backwards
        if (Input.GetKey("s"))
        {
            Move(-transform.forward, speed);
        }

        //Left
        if (Input.GetKey("a"))
        {
            Move(-transform.right, speed);
        }

        //Right
        if (Input.GetKey("d"))
        {
            Move(transform.right, speed);
        }

    }

    void Move(Vector3 direction, float speed)
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
