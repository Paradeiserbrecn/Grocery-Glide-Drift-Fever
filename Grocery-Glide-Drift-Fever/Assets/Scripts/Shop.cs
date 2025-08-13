using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shelfGroup;
    private HashSet<Item> _existingItems;
    private ShoppingList _shoppingList;
    void Start()
    {
        _existingItems = GatherItems();
        foreach (var item in _existingItems)
        {
            Debug.Log(item);
        }
    }

    private HashSet<Item> GatherItems()
    {
        HashSet<Item> items = new HashSet<Item>();
        foreach (ShelfScript shelf in shelfGroup.GetComponentsInChildren<ShelfScript>())
        {
            if (shelf.Item != null)
            {
                items.Add(shelf.Item);
            }
        }
        return items;
    }

    public List<Item> getExistingItems()
    {
        return new List<Item>(new List<Item>(_existingItems.ToArray()));
    }
}
