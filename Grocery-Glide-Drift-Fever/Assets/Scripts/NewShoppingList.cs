using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewShoppingList : MonoBehaviour
{
    [SerializeField] private Shop shop;
    private Dictionary<Item, int> _shoppingList;
    private Dictionary<Item, int> _cartCount;
    private Dictionary<Item, int> _boughtCount;
    
    [SerializeField] private GameObject listItemPrefab;
    private Dictionary<Item, UIListItemWrapper> _listItems = new Dictionary<Item, UIListItemWrapper>();
    
    
    [Header("DEBUG")]
    [SerializeField]private int listLength = 3;

    private void Start()
    {
        List<Item> existingItems = shop.getExistingItems();
        _shoppingList = RandomShoppingListValues(listLength, false);
        _cartCount = InitiateDictionary(existingItems);
        _boughtCount = InitiateDictionary(existingItems);
        
        LogDict(_shoppingList);
        
        InitiateUI();
        LogDict(_listItems);
        EventManager.ItemPickup += OnPickUp;
        EventManager.ItemDrop += OnDrop;
    }

    private void LogDict<T>(Dictionary<Item, T> dict)
    {
        Debug.Log("logging Dict:");
        foreach (var kvp in dict)
        {
            Debug.Log($"{kvp.Key.name} : {kvp.Value}");
        }
    }

    

    private Dictionary<Item, int> InitiateDictionary(List<Item> existingItems)
    {
        var dict = new Dictionary<Item, int>();
        foreach (Item item in existingItems)
        {
            dict[item] = 0;
        }
        return dict;
    }


    private Dictionary<Item, int> RandomShoppingListValues(int length, bool noDuplicates)
    {
        List<Item> items = shop.getExistingItems();
        Dictionary<Item, int> dict = InitiateDictionary(items);
        
        for (int i = 0; i < length; i++)
        {
            if (items.Count < 1)
            {
                Debug.Log("Not enough distinct items for shopping list of " + length + " length.");
                break;
            }
            
            int idx = Random.Range(0, items.Count);
            
            dict[items[idx]] += 1;
            if(noDuplicates) items.RemoveAt(idx);
        }
        return dict;
    }

    private void StyleUI(Item item)
    {
        UIListItemWrapper listItem = _listItems[item];
        int stillNeeded = _shoppingList[item] - _boughtCount[item];
        
        if (_cartCount[item] > 0)
        {
            listItem.gameObject.SetActive(true);
            if (_cartCount[item] <= _shoppingList[item])
            {
                listItem.SetText(_cartCount[item] + "/" +  stillNeeded + " " + item.ItemName);
            }
            else
            {
                listItem.SetText("<color=red>" + _cartCount[item] + "</color>/" +  stillNeeded + " " + item.ItemName);
            }
        }
        else 
        {
            if (stillNeeded <= 0)
            {
                listItem.gameObject.SetActive(false);
            }
            else
            {
                listItem.gameObject.SetActive(true);
                listItem.SetText(_cartCount[item] + "/" +  stillNeeded + " " + item.ItemName);
            }
        }
    }

    private void InitiateUI()
    {
        foreach (Item item in _shoppingList.Keys)
        {
            Debug.Log(item.ItemName);
            
            _listItems[item] = Instantiate(listItemPrefab, transform).GetComponent<UIListItemWrapper>();
            StyleUI(item);
        }
    }
    
    public void OnPickUp(Item item)
    {
        Debug.Log("OnPickUp");
        _cartCount[item]++;
        StyleUI(item);
    }


    public void OnDrop(Item item, bool buy)
    {
        _cartCount[item]--;
        if (buy)
        {
            _boughtCount[item]++;
        }
        StyleUI(item);
    }
}
