using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class camera : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform cart;
    [SerializeField] private float dist = 5.0f, height = 0.5f, smooth = 0.1f, lookheight = 1f;
    private Vector3 vel = Vector3.zero;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cartDir = -cart.forward;
        Vector3 globalCameraPos = cart.position + new Vector3(cartDir.x*dist,height,cartDir.z*dist);
        transform.position = Vector3.SmoothDamp(transform.position, globalCameraPos, ref vel, smooth);
        transform.LookAt(cart.position+cart.up*lookheight);
    }



}
