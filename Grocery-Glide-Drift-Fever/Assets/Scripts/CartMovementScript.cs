using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

[RequireComponent(typeof(Transform))]
public class CartMovement : MonoBehaviour
{
	public bool ragdoll = false;
	private bool _propUp = false;
	private Quaternion  _lastRot = Quaternion.identity;
	private Vector3 _vel = Vector3.zero;
	private Vector3 _rot = Vector3.zero;
	private Vector3 lastRotFactor = new Vector3(1, 0, 1);
	private Rigidbody _cart;
	[SerializeField] private float thrust = 100;
	[SerializeField] private float angular = 20;
	[SerializeField] private float physicalBaseWeight = 25;
	[SerializeField] private float weightMax = 100;
	[SerializeField] private float weight;
	[SerializeField] private WheelBehaviour[] wheels;
 	private float _tippingThreshold;
	[SerializeField] private float minDrift = 5;
 	[SerializeField] private float minBoost;
    [SerializeField] private float maxBoostStrength;
	private float _driftBoost = 1; 
 	[SerializeField] private float fixedTippingThreshold;
    [SerializeField] private float maxBoostedTippingThreshold;
	private float _driftValue; 
 	private float _driftScore;

	[SerializeField] private PhysicMaterial slipperyMaterial;
	[SerializeField] private PhysicMaterial ragdollMaterial;
	private BoxCollider _boxCollider;

	public bool IsDrifting { get; private set; } = false;
	public bool BoostReady { get; private set; } = false;
	private Vector3 propUpTargetPosition;

	
	private bool floorDetected;
	
	private float  _verticalAxis, _horizontalAxis;
	//private float _weightPenalty;


	private void Start()
	{
		_boxCollider = GetComponent<BoxCollider>();
		_cart = GetComponent<Rigidbody>();
		_lastRot = transform.rotation;
		_cart.maxAngularVelocity = 50;
		_tippingThreshold = fixedTippingThreshold;
		CheckTipping();
		UpdateWeight();
	}



	private void Update()
	{
		if (ragdoll && Input.GetKeyDown("r"))
		{
			propUpTargetPosition = transform.position + new Vector3(0, 1, 0);
			_cart.isKinematic = true;
			_propUp = true;
			// _lastRot.y = 0;
			_lastRot.x = 0;
			_lastRot.z = 0;
		}

		else{
			// Debug.Log(transform.rotation.x + " " + transform.rotation.y + " " + transform.rotation.z + " " + transform.rotation.w);
			//_weightPenalty = (3 - (weight / weightMax)) / 3;
			_verticalAxis = Input.GetAxisRaw("Vertical");
			_horizontalAxis = Input.GetAxisRaw("Horizontal");
			
			_driftValue = DriftValue();
			switch (_verticalAxis)
			{
				//thrust
				case > 0:
					_cart.AddForce(transform.forward * (_verticalAxis * thrust * _driftBoost * 3f));
					break;
				//brake
				case < 0:
					_cart.AddForce(transform.forward * (_verticalAxis * thrust * _driftBoost * 0.6f));
					break;
			}

			if (_horizontalAxis != 0) //rotation
			{
				_cart.AddTorque(transform.up * (_horizontalAxis * angular * 1f));
			}

			_driftValue = DriftValue();
			IsDrifting = CheckIsDrifting();
			CheckTipping();
			CheckCrashed();
			AddDriftScore();

			//Debug.Log(driftScore +" | "+ driftBoost + " | " + tippingThreshold);

			//interpolating values
			_tippingThreshold += (fixedTippingThreshold - _tippingThreshold) * 0.005f;
			_driftBoost += (1 - _driftBoost) * 0.01f;
		}
	}
    private void FixedUpdate()
    {
	    if (!ragdoll) return;
	    if (_propUp)
	    {
		    if (Vector3.Distance(transform.position, propUpTargetPosition) <= 0.02f && Quaternion.Angle(transform.rotation, _lastRot) <= 4f)
		    {
			    ActivateNormal();
			    return;
		    }
		    MakeUpright();
	    }
	    else
	    {
		    if (Vector3.Dot(transform.up, Vector3.up) > 0.9f && _cart.velocity.magnitude < 0.1f && _cart.angularVelocity.magnitude < 0.1f)
		    {
			    ActivateNormal();
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
		if (!ragdoll && _driftValue > minDrift)
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

	private void CheckCrashed()
	{
		if (Vector3.Dot(transform.up, Vector3.up) < 0.5f && _cart.velocity.magnitude < 1f && _cart.angularVelocity.magnitude < 0.1f)
		{
			floorDetected = Physics.Raycast(transform.position, transform.right , 5f, LayerMask.GetMask("Environment"));
			floorDetected = floorDetected || Physics.Raycast(transform.position, -transform.right , 5f, LayerMask.GetMask("Environment"));
			floorDetected = floorDetected || Physics.Raycast(transform.position, transform.forward , 5f, LayerMask.GetMask("Environment"));
			floorDetected = floorDetected || Physics.Raycast(transform.position, -transform.forward , 5f, LayerMask.GetMask("Environment"));
			floorDetected = floorDetected || Physics.Raycast(transform.position, transform.up, 5f, LayerMask.GetMask("Environment"));
			if (floorDetected)
			{
				ActivateRagdoll();
				Debug.Log("you crashed");
			}
		}
	}

	private void ActivateNormal()
	{
		ragdoll = false;
		_cart.isKinematic = false;
		_boxCollider.material = slipperyMaterial;
		_propUp = false;
	}

	private void ActivateRagdoll()
	{
		ragdoll = true;
		_boxCollider.material = ragdollMaterial;
	}

	private void AddDriftScore()
	{
		//Debug.Log("Drift Value: " + DriftValue() + ", Drift Score: " + _driftScore + ", drifitng: " + IsDrifting + ", boost ready: " + BoostReady);
		if (IsDrifting)
		{
			_driftScore += _driftValue * Time.deltaTime * 10;
			if (_driftScore > minBoost)
			{
				BoostReady = true;
			}
		}
		else
		{
			if (BoostReady)
			{
				_driftBoost += Mathf.Min(_driftScore / 350, maxBoostStrength);
				_tippingThreshold += Mathf.Min(_driftScore / 150, maxBoostedTippingThreshold);
			}

			_driftScore = 0;
			BoostReady = false;
		}
	}

	public void AddWeight(int add)
	{
		weight += add;
		UpdateWeight();
	}

	private void UpdateWeight()
	{
		_cart.mass = physicalBaseWeight + weight*0.2f;
	}
	
	private void MakeUpright() //raises the cart and changes its rotation to the last rotation before crashing
	{
		// Quaternion.RotateTowards approximates pi so we stop rotating once the angle becomes close to pi

		transform.position = Vector3.SmoothDamp(transform.position, propUpTargetPosition, ref _vel, 0.5f);
		// transform.forward = Vector3.SmoothDamp(transform.forward, _lastRot, ref _rot, 1f);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, _lastRot, 2f );
	}
}