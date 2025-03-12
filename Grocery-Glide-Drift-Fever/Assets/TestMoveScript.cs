using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 10f;
    private Vector3 velocity = Vector3.zero;

    
    void Update()
    {
        transform.position=Vector3.SmoothDamp(transform.position, target.position, ref velocity, speed);
    }
}
