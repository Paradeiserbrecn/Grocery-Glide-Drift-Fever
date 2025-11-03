using System;
using System.Collections.Generic;
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
        EventManager.ItemPickup += OnPickUp;
        EventManager.DropAll += OnDropAll;
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

    //searches the dictionary for an instance of the ListItem that is neither in the cart nor has been bought and updates it.
    //if it cant find one a new one is instanced
    public void OnPickUp(Item item)
    {
        LogItems(_items);
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

    //If buy searches for the first fitting ListItem in the cart and marks it as bought
    //if not searches the last fitting ListItem in the cart and deletes it or marks it as not in the cart if it's on the shoppinglist
    public void OnDrop(Item item, bool buy)
    {
        if (_items.ContainsKey(item) && _items[item].Count > 0)
        {
            ListItem bottomMostListItem = null;
 
            for (int i = _items[item].Count - 1; i >= 0; i--)
            { 
                ListItem listItem = _items[item][i]; 
                if (listItem.InCart) 
                {
                    if (listItem.InList == buy) 
                    {
                        bottomMostListItem = listItem; 
                        if(!buy) {break;}
                    } 
                    if (bottomMostListItem == null) bottomMostListItem = listItem;
                }
            }
            
            if (bottomMostListItem != null)
            {
                if(buy){
                    bottomMostListItem.Bought = true;
                    //TODO: evaluate purchase
                }

                if (bottomMostListItem.InList || bottomMostListItem.Bought)
                {
                    bottomMostListItem.InCart = false;
                    StyleListItem(bottomMostListItem, item);
                }
                else
                {
                    Destroy(bottomMostListItem.UIItemText.gameObject);
                    _items[item].Remove(bottomMostListItem);
                }
            }
        }
    }
    
    

    public void OnDropAll(bool buy)
    {
        foreach (Item item in _items.Keys)
        {
            //foreach (ListItem listItem in _items[item])
            //{
                OnDrop(item, buy);
            //}
        }
    }
}
