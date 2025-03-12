using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;

public class Cart : MonoBehaviour
{
    private bool _ragdoll = false, _isDrifting = false, _boostReady = false, _propUp = false;
    private Vector3 _lastRot, _vel = Vector3.zero;
    private Rigidbody _cart;
    [SerializeField] private float thrust = 100, angular = 20, weightMax = 100, mindrift = 5, weight;
    private float _driftBoost = 1, _tippingThreshold, _fixedTippingThreshold, _minBoost, _driftV, _driftScore;

    [SerializeField] PhysicMaterial slipperyM, ragdollM;
    private BoxCollider _boxCollider;


    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _cart = GetComponent<Rigidbody>();
        _lastRot = transform.forward;
        _cart.maxAngularVelocity = 50;
        _fixedTippingThreshold = thrust/7;
        _tippingThreshold = _fixedTippingThreshold;
        _minBoost = thrust * 2;
        CheckTipping();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!_ragdoll)
        {
            _lastRot = transform.forward;
            float weightPenalty = ((3 - (weight / weightMax)) / 3);
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");


            //Debug.Log(transform.up);
            _driftV = DriftValue();

            if (vertical > 0)  //thrust
            {
                _cart.AddForce(transform.forward * (vertical * thrust * _driftBoost * weightPenalty * 3f));
            }
            if (vertical < 0)  //brake
            {
                _cart.AddForce(transform.forward * (vertical * thrust * _driftBoost * weightPenalty * 0.6f));
            }
            if (horizontal != 0)  //rotation
            {
                _cart.AddTorque(transform.up * (horizontal * angular * weightPenalty * 1f));
            }

            _driftV = DriftValue();
            CheckDrifting();
            CheckTipping();
            AddDriftScore(weightPenalty);

            //Debug.Log(driftScore +" | "+ driftBoost + " | " + tippingThreshold);

            //interpolating values
            _tippingThreshold = _tippingThreshold + (_fixedTippingThreshold - _tippingThreshold) * 0.005f;
            _driftBoost = _driftBoost + (1 - _driftBoost) * 0.01f;
        }
        else
        {
            if (Input.GetKeyDown("r"))
            {
                _cart.isKinematic = true;
                _propUp = true;
                Debug.Log("R");
            }
            //TODO: detect being upright

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
        if (_driftV > _tippingThreshold)
        {
            ActivateRagdoll();
        }
    }

    private void CheckDrifting()
    {
        if (!_ragdoll && _driftV > mindrift)
        {
            _isDrifting = true;
        }
        else
        {
            _isDrifting = false;
        }
    }

    private void ActivateNormal()
    {
        _ragdoll = false;
        _cart.isKinematic = false;
        _boxCollider.material = slipperyM;
    }

    private void ActivateRagdoll()
    {
        _ragdoll = true;
        _boxCollider.material = ragdollM;
    }

    private void AddDriftScore(float weightPenalty)
    {
        if (_isDrifting)
        {
            _driftScore += _driftV * Time.deltaTime * 10 * weightPenalty;
            if (_driftScore > _minBoost)
            {
                _boostReady = true;
            }
        }
        else
        {
            if (_boostReady)
            {
                _driftBoost += Mathf.Min(_driftScore / 350, 1);
                _tippingThreshold += Mathf.Min(_driftScore / 150, 8);
            }
            _driftScore = 0;
            _boostReady = false;
        }

    }

    public bool GetIsDrifting()
    {
        return _isDrifting;
    }

    public bool GetBoostReady()
    {
        return _boostReady;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void MakeUpright() //raises the cart and changes its rotation to the last rotation before crashing
    {
        Debug.Log("doing stuff");
        transform.position = new Vector3(transform.position.x, Mathf.Min(transform.position.y + 0.02f, 2), transform.position.z);
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, 2, transform.position.z), ref vel, 0.5f);
        
        //Position = Position + (new Godot.Vector3(Position.X, 2, Position.Z) - Position) * (float)(10 * delta);
        //Rotation = Rotation + (lastOrientation - Rotation) * (float)(5 * delta);
    }
}
