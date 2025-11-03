using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EventManager
{
    //public static Action<string> testEvent;
    public static Action<Item> ItemPickup;
    public static Action<Item, bool> ItemDrop;
    public static Action<bool> DropAll;

    //public static void InvokeTestEvent(string whatYourMomTellsMe) => testEvent.Invoke(whatYourMomTellsMe);
    public static void InvokeItemPickup(Item item) => ItemPickup.Invoke(item);
    public static void InvokeDropAll(bool buy) => DropAll.Invoke(buy);
    
}
