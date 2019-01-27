using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform hand;
    public Transform rig;
    public float speed = 6f;
    public float rayDistance = .2f;
    public int raySteps = 5;
    public Animator anim;


    [Header("Controls")]
    string horizontal = "Horizontal";
    string vertical   = "Vertical";
    string pick       = "Pick";
    string interact   = "Interact";
        
    Rigidbody body;
    LayerMask maskItems;
    CapsuleCollider capsule;
    Item held = null;
    Item lastInteractedWIth = null;
    bool pickedUpThisFrame = false;

    void Awake()
    {
        maskItems = LayerMask.GetMask("Items");
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        Debug.Log(capsule);
    }

    private void Update()
    {
        Interact();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis(horizontal);
        float v = Input.GetAxis(vertical);

        Move(h, v);
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        // Move
        Vector3 movement = new Vector3(h, 0f, v);
        Vector3 direction = rig.transform.TransformDirection(Vector3.ClampMagnitude(movement, 1f));

        // movement = direction * speed * Time.deltaTime;
        float s = (held == null) ? speed : speed / ((held.size.x + held.size.y + held.size.z) * 0.333f);
        movement = direction * s;

        // body.MovePosition(transform.position + movement);
        body.velocity = new Vector3(movement.x, body.velocity.y, movement.z);

        // Rotate
        if(direction != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            body.MoveRotation(newRotation);
        }
    }

    void Interact()
    {
        float height = capsule.height * .5f;
        float distance = rayDistance + capsule.radius;
        Vector3 direction = transform.forward;
        Vector3 start = transform.position + new Vector3(0f, height, 0f);

        for (int i = 0; i<raySteps; i++)
        {
            RaycastHit hit;
            Vector3 step = start + (new Vector3(0f, - height / raySteps * i, 0f));
            if(Physics.Raycast(step, direction, out hit, distance, maskItems))
            {
                Debug.DrawRay(step, direction * distance, Color.green);

                Item item = hit.collider.GetComponent<Item>();
                if(item)
                {
                    InteractWithItem(item);
                    break;
                }
            } else
            {
                Debug.DrawRay(step, direction * distance, Color.blue);
            }
        }

        if (held != null && pickedUpThisFrame == false)
        {
            if (Input.GetButtonDown(pick))
            {
                held = held.PlaceOnTheFloor(transform.position + transform.forward * distance);
            }
        }

        if (Input.GetButtonUp(interact))
        {
            lastInteractedWIth = null;
        }

        pickedUpThisFrame = false;
    }

    void InteractWithItem(Item item)
    {
        if (Input.GetButton(interact))
        {
            item.Interact(this, item != lastInteractedWIth);
            lastInteractedWIth = item;
        }
        
        if (Input.GetButtonDown(pick))
        {
            Debug.Log(pick);

            if(held == null)
            {
                held = item.PickUp(this);
                pickedUpThisFrame = true;
            }
            else
            {
                held = held.PlaceInItem(item);
            }
        }
    }

    void Animating(float h, float v)
    {
        float speed = (new Vector2(h, v)).magnitude;
        anim.SetFloat("Speed", speed);
        anim.SetBool("isCarrying", held != null);
    }
}
