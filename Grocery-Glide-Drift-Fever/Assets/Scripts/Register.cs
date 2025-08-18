using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : MonoBehaviour
{
    [SerializeField] private ShoppingList shoppingList;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shoppingList.BuyAll(other.GetComponent<CartInventory>());
        }
    }
}
