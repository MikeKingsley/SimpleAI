using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 10f;
    public float sensitivityX = 5F;
    public float sensitivityY = 5F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;

    public bool flyCamera = false;

    enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    RotationAxes axes = RotationAxes.MouseXAndY;
    float rotationY = 0F;

    void Update()
    {

        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }

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
        if (!flyCamera)
            direction.y = 0;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
