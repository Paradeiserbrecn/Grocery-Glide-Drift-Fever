using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    [SerializeField] private CartMovement cart;
    private List<Item> inventory = new List<Item>();
    [SerializeField] private CapsuleCollider itemRange;

    [SerializeField] private ShoppingList shoppingList;

    private List<ShelfScript> collidingShelves = new List<ShelfScript>();

    private bool AddItem(Item item)
    {

        if (cart.AddWeight(item.Weight))
        {
            inventory.Add(item); 
            shoppingList.PickUp(item); 
            return true;
        }
        return false;
        //TODO instance prefab in cart
        //TODO change sound in audio
    }

    public void DropItem(Item item, bool buy)
    {
        inventory.Remove(item);
        cart.AddWeight(-item.Weight);
        shoppingList.Drop(item,buy);
        

        //TODO get rid of prefab in cart
        //TODO change sound in audio
    }

    public void DropAll(bool buy)
    {
        while(inventory.Count > 0)
        {
            DropItem(inventory[0], buy);
        }
    }

    public void PrintInv()
    {
        String text = "Inventory: \n";
        foreach (Item item in inventory)
        {
            text += (item.ItemName + ", ");
        }
        Debug.Log(text);
    }


    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (collidingShelves.Count > 0)
            {
                ShelfScript nearest = collidingShelves
                    .OrderBy(shelf => Vector3.Distance(shelf.transform.position, this.transform.position))
                    .First();
                if (nearest.hasItem)
                {
                    if(AddItem(nearest.Item)) nearest.TakeItem();
                    
                    //TODO: signify that the cart is full
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
        if (other.CompareTag("Shelf"))
        {
            collidingShelves.Add(other.GetComponentInParent<ShelfScript>());
            Debug.Log("entered: colliding with "+collidingShelves.Count+" shelves");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shelf"))
        { 
            collidingShelves.Remove(other.GetComponentInParent<ShelfScript>());
            Debug.Log("exited: colliding with "+collidingShelves.Count+" shelves");
        }
    }

    public List<Item> GetInventory()
    {
        return new List<Item>(inventory);
    }
}