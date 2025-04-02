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
	[SerializeField] private float ragdollDist = 10.0f;
	[SerializeField] private float height = 0.5f;
	[SerializeField] private float smooth = 0.1f;
	[SerializeField] private float lookHeight = 1f;
	private Vector3 _vel = Vector3.zero;
	private Vector3 _ragdollVel = Vector3.zero;
	private Vector3 _dirVel = Vector3.zero;
	private Vector3 _ragdollDirVel = Vector3.zero;
	private Vector3 _globalCameraSpacingVector;
	private Vector3 _globalCameraPos;
	private Vector3 _lookPoint;
	private Vector3 _direction;

	private RaycastHit hitInfo = new RaycastHit();
	private RaycastHit ragdolHitInfo = new RaycastHit();

	private void Start()
	{
		_cartTransform = cart.GetComponent<Transform>();
	}

	private void Update()
	{
		if (!cart.ragdoll)
		{
			_globalCameraSpacingVector = -_cartTransform.forward * dist;
			_globalCameraSpacingVector.y = height;

			_globalCameraPos = _cartTransform.position + _globalCameraSpacingVector;
			_direction =  _globalCameraPos - _lookPoint;
			
			//This makes the camera avoid terrain, so you do not end up looking at a wall
			_lookPoint = _cartTransform.position - _cartTransform.up * lookHeight;
			// Debug.DrawLine(_lookPoint, _globalCameraPos);
			bool deiMama = Physics.Raycast(_lookPoint, _direction , out hitInfo, _direction.magnitude, LayerMask.GetMask("Default"));
			
			if (deiMama)
			{
				//Debug.Log("Hallo freunde, ich darf doch freunde sagen?!");
				_globalCameraPos = hitInfo.point+ hitInfo.normal * 0.2f;
			}
			
			transform.position = Vector3.SmoothDamp(transform.position, _globalCameraPos, ref _vel, smooth);
			transform.forward = (_cartTransform.position - (transform.position + _cartTransform.up * lookHeight)).normalized;

		}
		else
		{
			_globalCameraPos =  _cartTransform.position + (transform.position - _cartTransform.position).normalized * ragdollDist;
			_direction = _globalCameraPos - _cartTransform.position;
			
			
			_lookPoint = _cartTransform.position - _cartTransform.up * lookHeight;
			
			Debug.Log("Dei mama is so groß wie des: " + (transform.position - _cartTransform.position));
			Debug.DrawLine(_lookPoint, _globalCameraPos,  Color.red);
			bool deiMama = Physics.Raycast(_lookPoint, _direction , out ragdolHitInfo, _direction.magnitude, LayerMask.GetMask("Default"));
			if (deiMama)
			{
				Debug.Log("Hallo freunde, ich darf doch freunde sagen?!");
				_globalCameraPos = ragdolHitInfo.point+ ragdolHitInfo.normal * 0.2f;
			}
			Debug.DrawLine(_lookPoint, _globalCameraPos, Color.blue);

			transform.position = Vector3.SmoothDamp(transform.position, _globalCameraPos, ref _ragdollVel, smooth);
			// transform.forward = (_cartTransform.position - (transform.position)).normalized;

			transform.forward = Vector3.SmoothDamp(transform.forward, (_cartTransform.position - (transform.position)).normalized, ref _ragdollDirVel, smooth);
		}

	}
}