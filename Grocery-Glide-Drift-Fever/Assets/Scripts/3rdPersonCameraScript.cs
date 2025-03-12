using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private CartMovement cart;
    private Transform cartTransfom;
    [SerializeField] private float dist = 5.0f;
    [SerializeField] private float ragdollDist = 1000.0f;
    [SerializeField] private float height = 0.5f;
    [SerializeField] private float smooth = 0.1f;
    [SerializeField] private float lookHeight = 1f;
    private Vector3 _vel = Vector3.zero;
    private Vector3 _cartDir;
    private Vector3 _globalCameraPos;

    void Start()
    {
        cartTransfom = cart.GetComponent<Transform>();
    }

    public void Update()
    {
        if (!cart._ragdoll){
            _cartDir = -cartTransfom.forward;
            _globalCameraPos = cartTransfom.position + new Vector3(_cartDir.x*dist,height,_cartDir.z*dist);
            transform.position = Vector3.SmoothDamp(transform.position, _globalCameraPos, ref _vel, smooth);
        }
        transform.LookAt(cartTransfom.position+cartTransfom.up*lookHeight);
        if(Vector3.Distance(transform.position, cartTransfom.position)>ragdollDist){
            transform.position = Vector3.SmoothDamp(transform.position, transform.position + (cartTransfom.position-transform.position).normalized * (Vector3.Distance(transform.position, cartTransfom.position)-ragdollDist) , ref _vel, smooth);
        }
        
        
    }
}
