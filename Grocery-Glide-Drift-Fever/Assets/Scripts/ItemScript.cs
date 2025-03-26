using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Item : MonoBehaviour
{
    
    public string  name { get => nameField; private set => nameField = value; }
    [SerializeField] private string nameField;
    
    public int weight { get => weightField; private set => weightField = value; }
    [SerializeField] private int weightField;
    
    public Mesh model { get => modelField; private set  => modelField = value; }
    [SerializeField] private Mesh modelField;
    
    public AudioClip audio { get => audioField; private set  => audioField = value; }
    [SerializeField] private AudioClip audioField;
    //[SerializeField] public ItemBehavior behavior;
}

