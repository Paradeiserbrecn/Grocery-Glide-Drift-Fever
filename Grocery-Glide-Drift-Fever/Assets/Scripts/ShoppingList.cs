using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShoppingList : MonoBehaviour
{
    [SerializeField] private Shop shop;
    private HashSet<Item> _shoppingList = new HashSet<Item>();
    private Dictionary<Item, ListItem> _items = new Dictionary<Item, ListItem>();
    [SerializeField] private GameObject listItemPrefab;
    
    [Header("DEBUG")]
    [SerializeField]private int listLength = 1;

    private void Start()
    {
        _shoppingList = MakeRandomList(listLength);
        PopulateItems();
    }


    private HashSet<Item> MakeRandomList(int length)
    {
        HashSet<Item> set = new HashSet<Item>();
        List<Item> remaining = shop.getExistingItems();
        for (int i = 0; i < length; i++)
        {
            if (remaining.Count < 1)
            {
                Debug.Log("Not enough distinct items for shopping list of " + length + " length.");
                break;
            }
            
            int idx = Random.Range(0, remaining.Count);
            set.Add(remaining[idx]);
            remaining.RemoveAt(idx);
        }
        return set;
    }

    private void PopulateItems()
    {
        foreach (Item item in _shoppingList)
        {
            GameObject listItemGameObject = Instantiate(listItemPrefab,this.transform);
            UIListItemWrapper listItemText = listItemGameObject.GetComponent<UIListItemWrapper>();
            ListItem newItem = new ListItem(listItemText);
            Debug.Log(listItemText);
            listItemText.SetText(item.ItemName, false);
            _items[item] = newItem;
        }
    }

    public void PickUp(Item item)
    {
        if (_items.ContainsKey(item))
        {
            _items[item].InCart = true;
            _items[item].UIItemText.SetColor(Color.gray);
        }
        
    }
    
    public void Drop(Item item)
    {
        if (_items.ContainsKey(item))
        {
            _items[item].InCart = false;
            _items[item].UIItemText.SetColor(Color.black);
        }
    }
    
    public void BuyAll(CartInventory inventory)
    {
        foreach (Item item in _items.Keys)
        {
            if (_items[item].InCart)
            {
                _items[item].InCart = false;
                _items[item].Bought = true;
                _items[item].UIItemText.SetText(item.ItemName,true);
                inventory.DropItem(item);
            }
        }
    }
}
