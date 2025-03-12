using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class CartMovement : MonoBehaviour
{
	public bool _ragdoll = false;
	private bool _propUp = false;
	private Vector3 _lastRot, _vel = Vector3.zero;
	private Rigidbody _cart;
	[SerializeField] private float thrust = 100;
	[SerializeField] private float angular = 20;
	[SerializeField] private float weightMax = 100;
	[SerializeField] private float minDrift = 5;
	[SerializeField] private float weight;
	private float _driftBoost = 1, _tippingThreshold, _fixedTippingThreshold, _minBoost, _driftValue, _driftScore;

	[SerializeField] private PhysicMaterial slipperyMaterial;
	[SerializeField] private PhysicMaterial ragdollMaterial;
	private BoxCollider _boxCollider;

	public bool IsDrifting { get; private set; } = false;
	public bool BoostReady { get; private set; } = false;
	[SerializeField] public Transform propupTargetPosition;
	private float propup = 0;



	private void Start()
	{
		_boxCollider = GetComponent<BoxCollider>();
		_cart = GetComponent<Rigidbody>();
		_lastRot = transform.forward;
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
			_cart.useGravity = false;
			_propUp = true;
			Debug.Log("R");
		}

		else{
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
			SetIsDrifting();
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
			ActivateRagdoll();
		}
	}

	private void SetIsDrifting()
	{
		IsDrifting = !_ragdoll && _driftValue > minDrift;
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
		Debug.Log("Make Upright Called " + Time.frameCount );
		transform.position=Vector3.SmoothDamp(transform.position, propupTargetPosition.position, ref _vel, 1f);
	}
}