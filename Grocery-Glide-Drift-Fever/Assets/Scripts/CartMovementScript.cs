using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

[RequireComponent(typeof(Transform))]
public class CartMovement : MonoBehaviour
{
	public bool _ragdoll = false;
	private bool _propUp = false;
	private Quaternion  _lastRot = Quaternion.identity;
	private Vector3 _vel = Vector3.zero;
	private Vector3 _rot = Vector3.zero;
	private Vector3 lastRotFactor = new Vector3(1, 0, 1);
	private Rigidbody _cart;
	[SerializeField] private float thrust = 100;
	[SerializeField] private float angular = 20;
	[SerializeField] private float weightMax = 100;
	[SerializeField] private float minDrift = 5;
	[SerializeField] private float weight;
	[SerializeField] private WheelBehaviour[] wheels;
	private float _driftBoost = 1, _tippingThreshold, _fixedTippingThreshold, _minBoost, _driftValue, _driftScore;

	[SerializeField] private PhysicMaterial slipperyMaterial;
	[SerializeField] private PhysicMaterial ragdollMaterial;
	private BoxCollider _boxCollider;

	public bool IsDrifting { get; private set; } = false;
	public bool BoostReady { get; private set; } = false;
	private Vector3 propUpTargetPosition;



	private void Start()
	{
		_boxCollider = GetComponent<BoxCollider>();
		_cart = GetComponent<Rigidbody>();
		_lastRot = transform.rotation;
		_cart.maxAngularVelocity = 50;
		_fixedTippingThreshold = thrust / 7;
		_tippingThreshold = _fixedTippingThreshold;
		_minBoost = thrust * 2;
		CheckTipping();
	}

	private float _weightPenalty, _verticalAxis, _horizontalAxis;

	private void Update()
	{
		if (_ragdoll && Input.GetKeyDown("r"))
		{
			propUpTargetPosition = transform.position + new Vector3(0, 1, 0);
			_cart.useGravity = false;
			_propUp = true;
			// _lastRot.y = 0;
			_lastRot.x = 0;
			_lastRot.z = 0;
		}

		else{
			// Debug.Log(transform.rotation.x + " " + transform.rotation.y + " " + transform.rotation.z + " " + transform.rotation.w);
			_weightPenalty = (3 - (weight / weightMax)) / 3;
			_verticalAxis = Input.GetAxisRaw("Vertical");
			_horizontalAxis = Input.GetAxisRaw("Horizontal");
			
			_driftValue = DriftValue();
			switch (_verticalAxis)
			{
				//thrust
				case > 0:
					_cart.AddForce(transform.forward * (_verticalAxis * thrust * _driftBoost * _weightPenalty * 3f));
					break;
				//brake
				case < 0:
					_cart.AddForce(transform.forward * (_verticalAxis * thrust * _driftBoost * _weightPenalty * 0.6f));
					break;
			}

			if (_horizontalAxis != 0) //rotation
			{
				_cart.AddTorque(transform.up * (_horizontalAxis * angular * _weightPenalty * 1f));
			}

			_driftValue = DriftValue();
			IsDrifting = CheckIsDrifting();
			CheckTipping();
			AddDriftScore(_weightPenalty);

			//Debug.Log(driftScore +" | "+ driftBoost + " | " + tippingThreshold);

			//interpolating values
			_tippingThreshold += (_fixedTippingThreshold - _tippingThreshold) * 0.005f;
			_driftBoost += (1 - _driftBoost) * 0.01f;
		}
	}
    private void FixedUpdate()
    {
        if (_ragdoll)
		{
			// {
			// 	//TODO: detect being upright
			// }

			if (_propUp)
			{
				MakeUpright();
			}
		}
    }

    private float DriftValue()
	{
		return Mathf.Abs(Vector3.Dot(transform.right, _cart.velocity) * ((1 + (weight / weightMax)) / 2));
	}

	private void CheckTipping()
	{
		if (_driftValue > _tippingThreshold)
		{
			_lastRot = transform.rotation;
			ActivateRagdoll();
		}
	}

	private bool CheckIsDrifting()
	{
		if (!_ragdoll && _driftValue > minDrift)
		{
			foreach (WheelBehaviour wheel in wheels)
			{
				wheel.PlaySmoke();
			}

			return true;
		}
		
		foreach (WheelBehaviour wheel in wheels)
		{
			wheel.StopSmoke();
		}
		return false;
	}

	private void ActivateNormal()
	{
		_ragdoll = false;
		_cart.isKinematic = false;
		_boxCollider.material = slipperyMaterial;
	}

	private void ActivateRagdoll()
	{
		_ragdoll = true;
		_boxCollider.material = ragdollMaterial;
	}

	private void AddDriftScore(float weightPenalty)
	{
		if (IsDrifting)
		{
			_driftScore += _driftValue * Time.deltaTime * 10 * weightPenalty;
			if (_driftScore > _minBoost)
			{
				BoostReady = true;
			}
		}
		else
		{
			if (BoostReady)
			{
				_driftBoost += Mathf.Min(_driftScore / 350, 1);
				_tippingThreshold += Mathf.Min(_driftScore / 150, 8);
			}

			_driftScore = 0;
			BoostReady = false;
		}
	}
	
	private void MakeUpright() //raises the cart and changes its rotation to the last rotation before crashing
	{
		// if (transform.rotation.Compare(_lastRot, 0) && transform.position.Compare(propUpTargetPosition, 0))
		// {
		// 	Debug.Log("Done Making upright");
		// 	return;
		// }
		transform.position = Vector3.SmoothDamp(transform.position, propUpTargetPosition, ref _vel, 1f);
		// transform.forward = Vector3.SmoothDamp(transform.forward, _lastRot, ref _rot, 1f);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, _lastRot, 1f );
	}
}