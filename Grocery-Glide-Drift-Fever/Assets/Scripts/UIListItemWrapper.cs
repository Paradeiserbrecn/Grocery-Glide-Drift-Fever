using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIListItemWrapper : MonoBehaviour
{
    private TMP_Text itemName;
    void Awake()
    {
        itemName = GetComponentInChildren<TMP_Text>();
    }

    public void SetColor(Color color)
    {
        itemName.color = color;
    }

    public void SetText(string text, bool strikethrough)
    {
        if (strikethrough) itemName.text = "<s>" + text + "</s>";
        else itemName.text = text;
    }
}
