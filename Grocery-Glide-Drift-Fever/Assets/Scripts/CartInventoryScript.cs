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
                collidingShelves.Sort(ShelfByDistanceComparator); 
                ShelfScript nearest = collidingShelves[0];
                if (nearest.hasItem)
                {
                    AddItem(nearest.Item);
                    nearest.TakeItem();
                }
            }
        }
    }

    /// <summary>
    /// Compares two shelves via their distance to the cart and follows the general CompareTo method functionality
    /// </summary>
    /// <param name="shelf">The shelf first shelf</param>
    /// <param name="other">The other shelf to compare `shelf` to</param>
    /// <returns> less than 0 if the distance to the cart of `shelf` is smaller than `other`.
    /// 0 if the distance to the cart of `shelf` is exactly the same as `other`.
    /// greater than 0 if the distance to the cart of `other` is greater than `shelf`</returns>
    private int ShelfByDistanceComparator(ShelfScript shelf, ShelfScript other)
    {
        return Vector3.Distance(shelf.transform.position, this.transform.position)
            .CompareTo(Vector3.Distance(other.transform.position, this.transform.position));
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
