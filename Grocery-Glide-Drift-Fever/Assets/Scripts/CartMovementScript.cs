using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Transform))]
public class CartMovement : MonoBehaviour
{
    #region Class Variables

    [Header("Components")] [SerializeField]
    private PhysicMaterial slipperyMaterial;

    [SerializeField] private PhysicMaterial ragdollMaterial;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private WheelBehaviour[] wheels;
    private FMOD.Studio.EventInstance fmodInstance;
    [SerializeField] private FMODUnity.EventReference fmodEvent;
    private Rigidbody cart;
    private BoxCollider boxCollider;
    private CapsuleCollider capsuleCollider;

    [Header("Properties")] [SerializeField]
    private float thrust = 100;

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
    private Vector3 vel = Vector3.zero;
    [Header("DEBUG")] public bool ragdoll = false;
    [SerializeField] private bool propUp = false;
    [SerializeField] private float tippingThreshold;
    [SerializeField] private float driftBoost = 1;
    [SerializeField] private float driftValue;
    [SerializeField] private float driftScore;
    [SerializeField] private float boostDecaySpeed;
    [SerializeField] private float tippingDecaySpeed;


    #region utility

    #endregion

    // Utility vars
    private Quaternion lastRot = Quaternion.identity;
    private Vector3 propUpTargetPosition;

    #endregion

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        cart = GetComponent<Rigidbody>();

        fmodInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        fmodInstance.start();

        lastRot = transform.rotation;
        cart.maxAngularVelocity = 50;
        tippingThreshold = fixedTippingThreshold;
        CheckTipping();
        UpdateWeight();

    }


    private float _verticalAxis, _horizontalAxis;

    private void Update()
    {
        fmodInstance.setParameterByName("speed", cart.velocity.magnitude);

        if (!ragdoll)
        {
            driftValue = DriftValue();
            IsDrifting = CheckIsDrifting();

            CheckUpright();
            CheckTipping();
            CheckCrashed();
            AddDriftScore();


            //interpolating values
            tippingThreshold += (fixedTippingThreshold - tippingThreshold) * tippingDecaySpeed * Time.deltaTime;
            driftBoost += (maxBoostStrength - driftBoost) * boostDecaySpeed * Time.deltaTime;

            fmodInstance.setParameterByName("drift boost", driftBoost);
            HandleBoostParticles();

            //Debug.Log(_cart.velocity.magnitude + ", " +_cart.angularVelocity.magnitude);
        }

        else if (Input.GetKeyDown("r"))
        {
            propUpTargetPosition = transform.position + new Vector3(0, 1, 0);
            cart.isKinematic = true;
            propUp = true;
            // _lastRot.y = 0;
            lastRot.x = 0;
            lastRot.z = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!ragdoll)
        {
            HandleInputAxisRaw();
            return;
        }
        if (propUp)
        {
            if (Vector3.Distance(transform.position, propUpTargetPosition) <= 0.02f &&
                Quaternion.Angle(transform.rotation, lastRot) <= 4f)
            {
                ActivateNormal();
                return;
            }

            MakeUpright();
        }
        else
        {
            if (Vector3.Dot(transform.up, Vector3.up) > 0.9f && cart.velocity.magnitude < 0.1f &&
                cart.angularVelocity.magnitude < 0.1f)
            {
                ActivateNormal();
            }
        }
    }

    private void HandleInputAxisRaw()
    {
        _verticalAxis = Input.GetAxisRaw("Vertical");
        _horizontalAxis = Input.GetAxisRaw("Horizontal");

        driftValue = DriftValue();
        switch (_verticalAxis)
        {
            //From my understanding Time.deltatime should not be required here as it is used in FixedUpdate... But somehow it is still required
            //thrust
            case > 0:
                cart.AddForce(transform.forward * (_verticalAxis * thrust * driftBoost * 110f * Time.deltaTime));
                break;
            //brake
            case < 0:
                cart.AddForce(transform.forward * (_verticalAxis * thrust * driftBoost * 40f * Time.deltaTime));
                break;
        }

        if (_horizontalAxis != 0) //rotation
        {
            cart.AddTorque(transform.up * (_horizontalAxis * angular * 50f * Time.deltaTime));
        }
    }

    private float DriftValue()
    {
        return Mathf.Abs(Vector3.Dot(transform.right, cart.velocity) * ((1 + (weight / weightMax)) / 2));
    }

    private void CheckTipping()
    {
        if (driftValue > tippingThreshold)
        {
            lastRot = transform.rotation;
            ActivateRagdoll();
        }
    }

    private bool CheckIsDrifting()
    {
        if (!ragdoll && driftValue > minDrift)
        {
            foreach (WheelBehaviour wheel in wheels)
            {
                wheel.PlaySmoke();
            }
            fmodInstance.setParameterByName("drift", 1);
            return true;
        }

        foreach (WheelBehaviour wheel in wheels)
        {
            wheel.StopSmoke();
            wheel.StopSpark();
        }

        fmodInstance.setParameterByName("drift", 0);
        return false;
    }

    private void CheckUpright()
    {
        IsUpright = Vector3.Dot(transform.up, Vector3.up) > 0.9f;
    }

    private bool _floorDetected;

    private void CheckCrashed()
    {
        if (!IsUpright)
        {
            _floorDetected = Physics.Raycast(raycastOrigin.position, raycastOrigin.right, 0.45f,
                LayerMask.GetMask("Environment"));
            Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + raycastOrigin.right * 0.45f, Color.red);
            _floorDetected = _floorDetected || Physics.Raycast(raycastOrigin.position, -raycastOrigin.right, 0.45f,
                LayerMask.GetMask("Environment"));
            Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + -raycastOrigin.right * 0.45f, Color.red);
            _floorDetected = _floorDetected || Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, 0.85f,
                LayerMask.GetMask("Environment"));
            Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + raycastOrigin.forward * 0.85f,
                Color.red);
            _floorDetected = _floorDetected || Physics.Raycast(raycastOrigin.position, -raycastOrigin.forward, 0.85f,
                LayerMask.GetMask("Environment"));
            Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + -raycastOrigin.forward * 0.85f,
                Color.red);
            _floorDetected = _floorDetected || Physics.Raycast(raycastOrigin.position, raycastOrigin.up, 0.45f,
                LayerMask.GetMask("Environment"));
            Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + raycastOrigin.up * 0.45f, Color.red);
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
        cart.isKinematic = false;
        boxCollider.material = slipperyMaterial;
        propUp = false;
        capsuleCollider.material = slipperyMaterial;
    }

    private void ActivateRagdoll()
    {
        ragdoll = true;
        boxCollider.material = ragdollMaterial;
        capsuleCollider.material = ragdollMaterial;
        IsDrifting = false;
        BoostReady = false;
        driftBoost = 1;
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
            driftScore += driftValue * Time.deltaTime * 10;
            if (driftScore > minBoost)
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
                driftBoost = 1 + Mathf.Min(driftScore * maxBoostStrength / 200, maxBoostStrength);
                tippingThreshold = fixedTippingThreshold +
                                    Mathf.Min(driftScore * maxTippingThresholdBoost / 200, maxTippingThresholdBoost);
                Debug.Log("boost: tipping threshold: min of(" + (driftScore * maxTippingThresholdBoost / 200) + ", " +
                          maxTippingThresholdBoost + "), driftBoost: " + driftBoost);
            }

            driftScore = 0;
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
        cart.mass = physicalBaseWeight + weight * 0.2f;
        fixedTippingThreshold =
            emptyTippingThreshold - ((emptyTippingThreshold - fullTippingThreshold) / weightMax * weight);
    }

    private void HandleBoostParticles()
    {
        if (driftBoost > 1.1f)
        {
            boostParticles.Play();
        }
        else
        {
            boostParticles.Stop();
        }
    }

    private void MakeUpright() //raises the cart and changes its rotation to the last rotation before crashing
    {
        // Quaternion.RotateTowards approximates pi so we stop rotating once the angle becomes close to pi
        transform.position = Vector3.SmoothDamp(transform.position, propUpTargetPosition, ref vel, 0.5f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lastRot, 2f);
    }
}