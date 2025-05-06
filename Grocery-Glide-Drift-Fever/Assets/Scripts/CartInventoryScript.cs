using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;

public class CartInventory : MonoBehaviour
{
    [SerializeField] private CartMovement cart;
    private ArrayList inventory = new ArrayList();
    [SerializeField] private CapsuleCollider itemRange;
    
    private List<ShelfScript> collidingShelves = new List<ShelfScript>();

    private void AddItem(Item item)
    {
        inventory.Add(item);
        cart.AddWeight(item.Weight);
        
        
        //TODO instance prefab in cart
        //TODO change sound in audio
    }

    private void DeleteItem(Item item)
    {
        inventory.Remove(item);
        cart.AddWeight(-item.Weight);

        //TODO get rid of prefab in cart
        //TODO change sound in audio
    }


    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (collidingShelves.Count > 0)
            {
                ShelfScript nearest = collidingShelves[0];
                float minDist = float.MinValue;
                if (collidingShelves.Count > 1)
                {
                    foreach (ShelfScript shelf in collidingShelves)
                    {
                        float dist = (shelf.GetPickupLocation().transform.position - this.transform.position).magnitude;
                        if (dist < minDist)
                        {
                            minDist = dist;
                            nearest = shelf;
                        }
                    }
                }
                if (nearest.hasItem)
                {
                    AddItem(nearest.GetItem());
                    nearest.TakeItem();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.GetComponentInParent<ShelfScript>());
        collidingShelves.Add(other.GetComponentInParent<ShelfScript>());
    }

    private void OnTriggerExit(Collider other)
    {
        collidingShelves.Remove(other.GetComponentInParent<ShelfScript>());
    }
}
