using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShoppingList : MonoBehaviour
{
    [SerializeField] private Shop shop;
    private Dictionary<Item, int> _shoppingList = new Dictionary<Item, int>();
    private Dictionary<Item,List<ListItem>> _items = new Dictionary<Item,List<ListItem>>();
    [SerializeField] private GameObject listItemPrefab;
    
    [Header("DEBUG")]
    [SerializeField]private int listLength = 3;

    private void Start()
    {
        _shoppingList = MakeRandomList(listLength, false);
        PopulateItems();
    }


    private Dictionary<Item, int> MakeRandomList(int length, bool noDuplicates)
    {
        Dictionary<Item,int> dict = new Dictionary<Item,int>();
        List<Item> remaining = shop.getExistingItems();
        for (int i = 0; i < length; i++)
        {
            if (remaining.Count < 1)
            {
                Debug.Log("Not enough distinct items for shopping list of " + length + " length.");
                break;
            }
            
            int idx = Random.Range(0, remaining.Count);
            if (!dict.ContainsKey(remaining[idx]))
            {
                dict[remaining[idx]] = 0;
            }
            dict[remaining[idx]] += 1;
            if(noDuplicates) remaining.RemoveAt(idx);
        }
        return dict;
    }

    private void LogShoppingDictionary(Dictionary<Item,int> dict)
    {
        String text = "Dict:= ";
        foreach (Item item in dict.Keys)
        {
            text += (item.ItemName + ": " + dict[item] + ",  ");
        }
        Debug.Log(text);
    }
    
    private void LogItems(Dictionary<Item,List<ListItem>> dict)
    {
        String text = "Dict:\n";
        foreach (Item item in dict.Keys)
        {
            foreach (ListItem listitem in dict[item])
            {
                text += (item.ItemName + ": " + listitem + "\n");
            }
        }
        Debug.Log(text);
    }

    private void PopulateItems()
    {
        foreach (Item item in _shoppingList.Keys)
        {
            for (int i = 0; i < _shoppingList[item]; i++)
            {
                StyleListItem(AddItem(item), item);
            }
        }
    }

    private ListItem AddItem(Item item)
    {
        GameObject listItemGameObject = Instantiate(listItemPrefab,this.transform);
        UIListItemWrapper listItemText = listItemGameObject.GetComponent<UIListItemWrapper>();
        ListItem newItem = new ListItem(listItemText);
        
        listItemText.SetText(item.ItemName, false);
        if (!_items.ContainsKey(item))
        {
            _items[item] = new List<ListItem>();
        }
        _items[item].Add(newItem);
        newItem.InList = _shoppingList[item] >= _items[item].Count;
        return newItem;
    }

    private void StyleListItem(ListItem listItem, Item item)
    {
        /*public bool InCart = false;
        public bool Bought = false;
        public bool InList = true;*/
        if (listItem.Bought)
        {
            if (listItem.InList)
            {
                listItem.UIItemText.SetText(item.ItemName, true);
                listItem.UIItemText.SetColor(Color.green);
            }
            else
            {
                listItem.UIItemText.SetText(item.ItemName + "?", true);
                listItem.UIItemText.SetColor(Color.red);
            }
        }

        else if (listItem.InCart)
        {
            if (listItem.InList)
            {
                listItem.UIItemText.SetText(item.ItemName, false);
                listItem.UIItemText.SetColor(Color.gray);
            }
            else{
                listItem.UIItemText.SetText(item.ItemName + "?", false);
                listItem.UIItemText.SetColor(Color.red);
            }
        }
        
        else
        {
            listItem.UIItemText.SetText(item.ItemName, false);
            listItem.UIItemText.SetColor(Color.black);
        }
    }

    //searches the dictionary for an instance of the item that is neither in the cart nor has been bought and updates it.
    //if it cant find one a new one is instanced
    public void PickUp(Item item)
    {
        if (_items.ContainsKey(item))
        {
            foreach (ListItem listItem in _items[item])
            {
                if (!listItem.InCart && !listItem.Bought)
                {
                    listItem.InCart = true;
                    StyleListItem(listItem, item);
                    return;
                }
            }
        }
        ListItem newListItem = AddItem(item);
        newListItem.InCart = true;
        StyleListItem(newListItem, item);
    }

    public void Drop(Item item, bool buy)
    {
        if (_items.ContainsKey(item) &&  _items[item].Count > 0)
        {
            foreach (ListItem listItem in _items[item])
            {
                if (listItem.InCart)
                {
                    if (buy &&  !listItem.Bought){ 
                        listItem.Bought = true;
                        
                        //TODO: evaluate purchase 
                    }
                    
                    if (listItem.InList || listItem.Bought)
                    {
                        listItem.InCart = false;
                        StyleListItem(listItem, item);
                    }
                    else
                    {
                        Destroy(listItem.UIItemText.gameObject);
                        _items[item].Remove(listItem);
                    }
                    return;
                }
            }
        }
    }
    
    public void BuyAll(CartInventory inventory)
    {
        inventory.DropAll(true);
    }
}
