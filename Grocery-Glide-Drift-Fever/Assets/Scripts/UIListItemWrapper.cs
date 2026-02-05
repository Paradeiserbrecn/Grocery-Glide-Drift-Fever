using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIListItemWrapper : MonoBehaviour
{
    private TMP_Text itemName;
    void Awake()
    {
        itemName = GetComponent<TMP_Text>();
    }

    public void SetColor(Color color)
    {
        itemName.color = color;
    }

    public void SetText(string text)
    {
        itemName.text = text;
    }

}
