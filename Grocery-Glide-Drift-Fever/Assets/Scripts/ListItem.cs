using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class ListItem
{
    public bool InCart = false;
    public bool Bought = false;
    public bool InList = true;
    public UIListItemWrapper UIItemText;

    public ListItem(UIListItemWrapper uiListItem)
    {
        this.UIItemText = uiListItem;
    }

    public override String ToString()
    {
        return ("InCart: " + InCart + ", Bought: " + Bought + ", InList: "+ InList);
    }
}
