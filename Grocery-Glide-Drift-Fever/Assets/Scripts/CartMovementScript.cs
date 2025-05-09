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
	[SerializeField] private ParticleSystem boostParticles;
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
    [SerializeField] private float emptyTippingThreshold;
    [SerializeField] private float fullTippingThreshold;
 	[SerializeField] private float fixedTippingThreshold;
    [SerializeField] private float maxTippingThresholdBoost;
	public bool IsDrifting { get; private set; } = false;
	public bool BoostReady { get; private set; } = false;
	public bool IsUpright { get; private set; } = false;
	private Vector3 _vel = Vector3.zero;
	[Header("DEBUG")]
	public bool ragdoll = false;
	[SerializeField] private bool _propUp = false;
 	[SerializeField] private float _tippingThreshold;
	[SerializeField] private float _driftBoost = 1; 
	[SerializeField] private float _driftValue; 
 	[SerializeField] private float _driftScore;
	[SerializeField] private float _boostDecaySpeed;
	[SerializeField] private float _tippingDecaySpeed;


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
			_verticalAxis = Input.GetAxisRaw("Vertical");
			_horizontalAxis = Input.GetAxisRaw("Horizontal");
			
			_driftValue = DriftValue();
			switch (_verticalAxis)
			{
				//thrust
				case > 0:
					_cart.AddForce(transform.forward * (_verticalAxis * thrust * _driftBoost * 110f * Time.deltaTime));
					break;
				//brake
				case < 0:
					_cart.AddForce(transform.forward * (_verticalAxis * thrust * _driftBoost * 40f * Time.deltaTime));
					break;
			}

			if (_horizontalAxis != 0) //rotation
			{
				_cart.AddTorque(transform.up * (_horizontalAxis * angular * 50f * Time.deltaTime));
			}

			_driftValue = DriftValue();
			IsDrifting = CheckIsDrifting();

			CheckUpright();
			CheckTipping();
			CheckCrashed();
			AddDriftScore();


			//interpolating values
			_tippingThreshold += (fixedTippingThreshold - _tippingThreshold) * _tippingDecaySpeed * Time.deltaTime;
			_driftBoost += (maxBoostStrength - _driftBoost) * _boostDecaySpeed * Time.deltaTime;

			HandleBoostParticles();

			
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
			wheel.StopSpark();
		}
		return false;
	}

	private void CheckUpright(){
		IsUpright = Vector3.Dot(transform.up, Vector3.up) > 0.9f;
	}

	private bool _floorDetected;
	private void CheckCrashed()
	{
		if (!IsUpright)
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
		boostParticles.Stop();
		boostParticles.Clear();
		foreach (WheelBehaviour wheel in wheels)
		{
			wheel.StopSmoke();
			wheel.StopSpark();
		}
	}

	private void AddDriftScore()
	{
		if (IsDrifting)
		{
			_driftScore += _driftValue * Time.deltaTime * 10;
			if (_driftScore > minBoost)
			{
				BoostReady = true;
				foreach (WheelBehaviour wheel in wheels)
				{
					wheel.PlaySpark();
				}
			}
		}
		else
		{
			if (BoostReady)
			{
				_driftBoost = 1  + Mathf.Min(_driftScore * maxBoostStrength / 200, maxBoostStrength);
				_tippingThreshold = fixedTippingThreshold + Mathf.Min(_driftScore * maxTippingThresholdBoost / 200, maxTippingThresholdBoost);
				Debug.Log("boost: tipping threshold: min of(" + (_driftScore * maxTippingThresholdBoost / 200) + ", " + maxTippingThresholdBoost + "), driftBoost: " + _driftBoost);
			}

			_driftScore = 0;
			BoostReady = false;
		}
	}

	public void AddWeight(int add)
	{
		if (weight + add > weightMax) return;
		
		weight += add;
		UpdateWeight();
	}

	private void UpdateWeight()
	{
		_cart.mass = physicalBaseWeight + weight*0.2f;
		fixedTippingThreshold = emptyTippingThreshold - ((emptyTippingThreshold - fullTippingThreshold)/weightMax * weight);
	}

	private void HandleBoostParticles(){
		if (_driftBoost > 1.1f){
			boostParticles.Play();
		}
		else{
			boostParticles.Stop();
		}
	}
	
	private void MakeUpright() //raises the cart and changes its rotation to the last rotation before crashing
	{
		// Quaternion.RotateTowards approximates pi so we stop rotating once the angle becomes close to pi

		transform.position = Vector3.SmoothDamp(transform.position, _propUpTargetPosition, ref _vel, 0.5f);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, _lastRot, 2f );
	}
}