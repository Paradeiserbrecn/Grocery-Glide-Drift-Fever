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
    public Item Item => item;
    [SerializeField] private Transform pickupLocation;
    public Transform PickupLocation => pickupLocation;

    [Header("Debug")] [SerializeField] private int spinSpeed;
    [SerializeField] private float height;
    [SerializeField] private float heightVariance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float restockTime;

    private Transform _itemTransform;
    private Transform _meshTransform;
    private GameObject _itemInstance;
    private Vector3 _vel = Vector3.zero;
    private Vector3 _upPoint;
    private Vector3 _downPoint;
    private bool _animationSmoothDampUpwards = true;

    [NonSerialized] public bool hasItem;

    private void Start()
    {
        if (item != null) InitializeShelfItem();
    }

    private void InitializeShelfItem()
    {
        cone.SetActive(true);
        _itemInstance = Instantiate(item.Model, itemSpawn.transform, false);
        _itemTransform = itemSpawn.transform;
        _meshTransform = _itemInstance.transform;
        _itemInstance.transform.localScale *= 1.5f;
        _itemInstance.GetComponent<MeshRenderer>().material = hologramMaterial;
        itemSpawn.transform.position =
            new Vector3(itemSpawn.transform.position.x, height, itemSpawn.transform.position.z);
        _upPoint = itemSpawn.transform.position + Vector3.up * heightVariance;
        _downPoint = itemSpawn.transform.position + Vector3.down * heightVariance;
        hasItem = true;
    }

    private void FixedUpdate()
    {
        if (!hasItem) return;

        if (_animationSmoothDampUpwards)
        {
            _itemTransform.position = Vector3.SmoothDamp(_itemTransform.position, _upPoint, ref _vel, moveSpeed);
            if ((_itemTransform.position - _upPoint).magnitude < 0.1f) _animationSmoothDampUpwards = false;
        }
        else
        {
            _itemTransform.position = Vector3.SmoothDamp(_itemTransform.position, _downPoint, ref _vel, moveSpeed);
            if ((_itemTransform.position - _downPoint).magnitude < 0.1f) _animationSmoothDampUpwards = true;
        }

        _meshTransform.Rotate(Vector3.up, spinSpeed);
    }

    /// <summary>
    /// Deactivates the cone and itemSpawn GameObjects and automatically restocks the item after `restockTime` seconds
    /// </summary>
    public void TakeItem()
    {
        cone.SetActive(false);
        itemSpawn.SetActive(false);
        hasItem = false;
        StartCoroutine(Restock());
        return;

        // We only ever want to be able to restock if the item has been taken out of the shelf
        IEnumerator Restock()
        {
            yield return new WaitForSeconds(restockTime);

            cone.SetActive(true);
            itemSpawn.SetActive(true);
            hasItem = true;
        }
    }
}