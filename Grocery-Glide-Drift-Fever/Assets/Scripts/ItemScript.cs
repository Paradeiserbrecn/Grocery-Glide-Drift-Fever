using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items")]
public class Item : ScriptableObject
{
    
    public string  ItemName { get => nameField; private set => nameField = value; }
    [SerializeField] private string nameField;
    
    public int Weight { get => weightField; private set => weightField = value; }
    [SerializeField] private int weightField;
    
    public GameObject Model { get => modelField; private set  => modelField = value; }
    [SerializeField] private GameObject modelField;
    
    public AudioClip Audio { get => audioField; private set  => audioField = value; }
    [SerializeField] private AudioClip audioField;
    //[SerializeField] public ItemBehavior behavior;
}

