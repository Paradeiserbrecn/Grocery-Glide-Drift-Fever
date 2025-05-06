using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ShelfScript : MonoBehaviour
{
    [SerializeField] private Material hologramMaterial;
    [SerializeField] private GameObject itemSpawn;
    [SerializeField] private GameObject cone;
    [SerializeField] private Item item;
    [SerializeField] private Transform pickupLocation;

    [Header("Debug")] 
    [SerializeField] private int spinSpeed;
    [SerializeField] private float height;
    [SerializeField] private float heightVariance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float restockTime;

    private Transform itemTransform;
    private Transform meshTransform;
    private GameObject itemInstance;
    private Vector3 _vel = Vector3.zero;
    private Vector3 upPoint;
    private Vector3 downPoint;
    private bool up = true;

    public bool hasItem = false;
    
    private void Start()
    {
        if (item == null) return;
        
        cone.SetActive(true);
        itemInstance = Instantiate(item.Model, itemSpawn.transform, false);
        itemTransform = itemSpawn.transform;
        meshTransform = itemInstance.transform;
        itemInstance.transform.localScale *= 1.5f;
        itemInstance.GetComponent<MeshRenderer>().material = hologramMaterial;
        itemSpawn.transform.position =  new Vector3(itemSpawn.transform.position.x, height, itemSpawn.transform.position.z);
        upPoint = itemSpawn.transform.position + Vector3.up * heightVariance;
        downPoint = itemSpawn.transform.position + Vector3.down * heightVariance;
        hasItem = true;
    }

    private void FixedUpdate()
    {
        if (!hasItem) return;
        
        if (up)
        {
            itemTransform.position = Vector3.SmoothDamp(itemTransform.position, upPoint, ref _vel, moveSpeed);
            if((itemTransform.position - upPoint).magnitude < 0.1f){up = false;}
        }
        else
        {
            itemTransform.position = Vector3.SmoothDamp(itemTransform.position, downPoint, ref _vel, moveSpeed);
            if((itemTransform.position - downPoint).magnitude < 0.1f){up = true;}
        }
        
        
        meshTransform.Rotate(Vector3.up, spinSpeed);
    }

    public Item GetItem()
    {
        return item;
    }

    public Transform GetPickupLocation()
    {
        return pickupLocation;
    }

    public void TakeItem()
    {
        cone.SetActive(false);
        hasItem = false;
        StartCoroutine(Restock());
    }

    private void RestockItem()
    {
        cone.SetActive(true);
        hasItem = true;
    }
    
    
    
    IEnumerator Restock()
    {
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        yield return new WaitForSeconds(restockTime);

        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        
        RestockItem();
    }
}
