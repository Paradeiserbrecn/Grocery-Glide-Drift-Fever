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
	private Transform _cartTransform;
	[SerializeField] private float dist = 5.0f;
	[SerializeField] private float ragdollDist = 1000.0f;
	[SerializeField] private float height = 0.5f;
	[SerializeField] private float smooth = 0.1f;
	[SerializeField] private float lookHeight = 1f;
	private Vector3 _vel = Vector3.zero;
	private Vector3 _dirVel = Vector3.zero;
	private Vector3 _globalCameraSpacingVector;
	private Vector3 _globalCameraPos;

	private void Start()
	{
		_cartTransform = cart.GetComponent<Transform>();
	}

	private void Update()
	{
		if (!cart._ragdoll)
		{
			_globalCameraSpacingVector = -_cartTransform.forward * dist;
			_globalCameraSpacingVector.y = height;

			_globalCameraPos = _cartTransform.position + _globalCameraSpacingVector;
			transform.position = Vector3.SmoothDamp(transform.position, _globalCameraPos, ref _vel, smooth);
			
			transform.forward = (_cartTransform.position - (transform.position + _cartTransform.up * lookHeight)).normalized;

		}
		else
		{
			transform.position += (_cartTransform.position - transform.position).normalized *
			                      (Vector3.Distance(transform.position, _cartTransform.position) - ragdollDist);
			
			
			transform.forward = Vector3.SmoothDamp(transform.forward, (_cartTransform.position - (transform.position + _cartTransform.up * lookHeight)).normalized, ref _dirVel, smooth);
		}

	}
}