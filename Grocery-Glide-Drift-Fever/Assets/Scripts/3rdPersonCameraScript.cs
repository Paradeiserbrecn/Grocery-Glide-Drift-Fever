using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform cart;
    [SerializeField] private float dist = 5.0f;
    [SerializeField] private float height = 0.5f;
    [SerializeField] private float smooth = 0.1f;
    [SerializeField] private float lookHeight = 1f;
    private Vector3 _vel = Vector3.zero;
    
    private Vector3 _cartDir;
    private Vector3 _globalCameraPos;
    
    public void Update()
    {
        _cartDir = -cart.forward;
        _globalCameraPos = cart.position + new Vector3(_cartDir.x*dist,height,_cartDir.z*dist);
        transform.position = Vector3.SmoothDamp(transform.position, _globalCameraPos, ref _vel, smooth);
        transform.LookAt(cart.position+cart.up*lookHeight);
    }



}
