using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shelfGroup;
    private HashSet<Item> _existingItems;
    void Awake()
    {
        _existingItems = GatherItems();
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

    private void LogItems()
    {
        foreach (Item item in _existingItems)
        {
            Debug.Log(item.ToString());
        }
    }
}
