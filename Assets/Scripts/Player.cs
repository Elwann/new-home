using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform main;
    public Transform rig;
    public float speed = 6f;            
    Rigidbody body;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);

        Animating(h, v);
    }

    void Move(float h, float v)
    {
        // Move

        Vector3 movement = new Vector3(h, 0f, v);
        Vector3 direction = rig.transform.TransformDirection(movement.normalized);

        movement = direction * speed * Time.deltaTime;

        body.MovePosition(transform.position + movement);

        // Rotate

        if(direction != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            body.MoveRotation(newRotation);
        }
    }

    void Animating(float h, float v)
    {

    }
}
