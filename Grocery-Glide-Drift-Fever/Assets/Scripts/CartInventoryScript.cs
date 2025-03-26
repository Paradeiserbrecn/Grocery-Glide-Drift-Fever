using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CartInventory : MonoBehaviour
{
    private CartMovement cart;
    private ArrayList inventory = new ArrayList();

    private void AddItem(Item item)
    {
        inventory.Add(item);
        cart.AddWeight(item.weight);
        
        //TODO instance prefab in cart
        //TODO change sound in audio
    }

    private void DeleteItem(Item item)
    {
        inventory.Remove(item);
        cart.AddWeight(-item.weight);

        //TODO get rid of prefab in cart
        //TODO change sound in audio
    }
}
