using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector3Int size = new Vector3Int(1, 1, 1);

    public bool isContainer = false;
    public bool isHeavy = false;
    public bool isOpen = false;
    public List<Item> contains = new List<Item>();

    public Vector3Int contentSize = new Vector3Int();
    public int rows = 1;

    Animator anim;

    bool held = false;
    Player parent = null;
    Item container = null;


    Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        transform.position = Manager.instance.room.FindPlace(this, transform.position);
    }

    public virtual void Interact(Player player, bool firstFrame)
    {
        if (!isContainer || !firstFrame)
            return;


        isOpen = !isOpen;
        Debug.Log("Is open : " + isOpen);
        if (anim != null)
            anim.SetBool("isOpen", isOpen);
    }

    public virtual Item PickUp(Player player)
    {
        if(isOpen && contains.Count != 0)
        {
            return contains[contains.Count - 1].PickUp(player);
        }

        if(isHeavy)
        {
            //TODO: Gestion de déplacement des objets lourds
            return null;
        }

        if(container != null)
        {
            container.RemoveFromContent(this);
            container = null;
        }

        held = true;
        parent = player;
        
        transform.SetParent(player.hand.transform);

        transform.localPosition = new Vector3(-size.x * 0.5f * Room.gridSize, 0f, 0f);
        transform.localRotation = new Quaternion();

        return this;
    }

    public virtual Item PlaceOnTheFloor(Vector3 position)
    {
        Vector3 pos = Manager.instance.room.FindPlace(this, position);

        if(pos == Vector3.zero)
            return this;
        
        transform.SetParent(null);

        transform.localPosition = pos;
        transform.localRotation = new Quaternion();

        transform.SetParent(null);

        return null;
    }

    public virtual Item PlaceInItem(Item item)
    {
        if (isContainer)
        {
            if(item.isContainer && item.isOpen)
                return this;

            return PlaceOnTheFloor(item.transform.position);
        }

        if (!item.isContainer || !item.isOpen)
            return this;

        return item.AddToContent(item);
    }

    public virtual Item AddToContent(Item item)
    {
        if (!isContainer || !isOpen)
            return item;



        return null;
    }

    public virtual void RemoveFromContent(Item item)
    {
        contains.Remove(item);

    }
}
