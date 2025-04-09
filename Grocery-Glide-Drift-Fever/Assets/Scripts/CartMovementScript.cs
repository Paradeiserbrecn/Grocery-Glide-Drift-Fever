using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

[RequireComponent(typeof(Transform))]
public class CartMovement : MonoBehaviour
{
	#region Class Variables
	[Header("Components")]
	[SerializeField] private PhysicMaterial slipperyMaterial;
	[SerializeField] private PhysicMaterial ragdollMaterial;
	[SerializeField] private Transform _raycastOrigin;
	[SerializeField] private WheelBehaviour[] wheels;
	private Rigidbody _cart;
	private BoxCollider _boxCollider;
	private CapsuleCollider _capsuleCollider;
	[Header("Properties")]
	[SerializeField] private float thrust = 100;
	[SerializeField] private float angular = 20;
	[SerializeField] private float physicalBaseWeight = 25;
	[SerializeField] private float weightMax = 100;
	[SerializeField] private float weight;
	[SerializeField] private float minDrift = 5;
 	[SerializeField] private float minBoost;
    [SerializeField] private float maxBoostStrength;
 	[SerializeField] private float fixedTippingThreshold;
    [SerializeField] private float maxBoostedTippingThreshold;
	public bool IsDrifting { get; private set; } = false;
	public bool BoostReady { get; private set; } = false;
	private Vector3 _vel = Vector3.zero;
	[Header("DEBUG")]
	public bool ragdoll = false;
	[SerializeField] private bool _propUp = false;
 	[SerializeField] private float _tippingThreshold;
	[SerializeField] private float _driftBoost = 1; 
	[SerializeField] private float _driftValue; 
 	[SerializeField] private float _driftScore;

	#region utility

	#endregion
	// Utility vars
	private Quaternion  _lastRot = Quaternion.identity;
	private Vector3 _propUpTargetPosition;
	#endregion

	private void Start()
	{
		_boxCollider = GetComponent<BoxCollider>();
		_capsuleCollider = GetComponent<CapsuleCollider>();
		_cart = GetComponent<Rigidbody>();
		_lastRot = transform.rotation;
		_cart.maxAngularVelocity = 50;
		_tippingThreshold = fixedTippingThreshold;
		CheckTipping();
		UpdateWeight();
	}


	private float  _verticalAxis, _horizontalAxis;	
	private void Update()
	{
		if(!ragdoll){
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


			//interpolating values
			_tippingThreshold += (fixedTippingThreshold - _tippingThreshold) * 0.005f;
			_driftBoost += (1 - _driftBoost) * 0.01f;

			//Debug.Log(_cart.velocity.magnitude + ", " +_cart.angularVelocity.magnitude);
		}

		else if(Input.GetKeyDown("r")){
			_propUpTargetPosition = transform.position + new Vector3(0, 1, 0);
			_cart.isKinematic = true;
			_propUp = true;
			// _lastRot.y = 0;
			_lastRot.x = 0;
			_lastRot.z = 0;
		}
	}
    private void FixedUpdate()
    {
	    if (!ragdoll) return;
	    if (_propUp)
	    {
		    if (Vector3.Distance(transform.position, _propUpTargetPosition) <= 0.02f && Quaternion.Angle(transform.rotation, _lastRot) <= 4f)
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

	private bool _floorDetected;
	private void CheckCrashed()
	{
		if (Vector3.Dot(transform.up, Vector3.up) < 0.9f)
		{

			_floorDetected = Physics.Raycast(_raycastOrigin.position, _raycastOrigin.right , 0.45f, LayerMask.GetMask("Environment"));
			Debug.DrawLine(_raycastOrigin.position, _raycastOrigin.position + _raycastOrigin.right * 0.45f,  Color.red);
			_floorDetected = _floorDetected || Physics.Raycast(_raycastOrigin.position, -_raycastOrigin.right , 0.45f, LayerMask.GetMask("Environment"));
			Debug.DrawLine(_raycastOrigin.position, _raycastOrigin.position + -_raycastOrigin.right * 0.45f,  Color.red);
			_floorDetected = _floorDetected || Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward , 0.85f, LayerMask.GetMask("Environment"));
			Debug.DrawLine(_raycastOrigin.position, _raycastOrigin.position + _raycastOrigin.forward * 0.85f,  Color.red);
			_floorDetected = _floorDetected || Physics.Raycast(_raycastOrigin.position, -_raycastOrigin.forward , 0.85f, LayerMask.GetMask("Environment"));
			Debug.DrawLine(_raycastOrigin.position, _raycastOrigin.position + -_raycastOrigin.forward * 0.85f,  Color.red);
			_floorDetected = _floorDetected || Physics.Raycast(_raycastOrigin.position, _raycastOrigin.up, 0.45f, LayerMask.GetMask("Environment"));
			Debug.DrawLine(_raycastOrigin.position, _raycastOrigin.position + _raycastOrigin.up * 0.45f,  Color.red);
			if (_floorDetected)
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
		_capsuleCollider.material =slipperyMaterial;
	}

	private void ActivateRagdoll()
	{
		ragdoll = true;
		_boxCollider.material = ragdollMaterial;
		_capsuleCollider.material = ragdollMaterial;
		IsDrifting = false;
		BoostReady = false;
	}

	private void AddDriftScore()
	{
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

		transform.position = Vector3.SmoothDamp(transform.position, _propUpTargetPosition, ref _vel, 0.5f);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, _lastRot, 2f );
	}
}