using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListItem
{
    public bool InCart = false;
    public bool Bought = false;
    public UIListItemWrapper UIItemText;

    public ListItem(UIListItemWrapper uiListItem)
    {
        this.UIItemText = uiListItem;
    }
}
