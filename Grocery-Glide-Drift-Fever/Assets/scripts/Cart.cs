using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;

public class Cart : MonoBehaviour
{
    private bool ragdoll = false, isDrifting = false, boostReady = false, propUp = false;
    private Vector3 lastRot, vel = Vector3.zero;
    private Rigidbody cart;
    [SerializeField] private float thrust = 100, angular = 20, weightMax = 100, mindrift = 5, weight;
    private float driftBoost = 1, tippingThreshold, fixedTippingThreshold, minBoost, driftV, driftScore;

    [SerializeField] PhysicMaterial slipperyM, ragdollM;


    void Start()
    {
        cart = GetComponent<Rigidbody>();
        lastRot = transform.forward;
        cart.maxAngularVelocity = 50;
        fixedTippingThreshold = thrust/7;
        tippingThreshold = fixedTippingThreshold;
        minBoost = thrust * 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!ragdoll)
        {
            lastRot = transform.forward;
            float weightPenalty = ((3 - (weight / weightMax)) / 3);
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");


            //Debug.Log(transform.up);
            driftV = driftValue();

            if (vertical > 0)  //thrust
            {
                cart.AddForce(transform.forward * vertical * thrust  * driftBoost * weightPenalty * 3f);
            }
            if (vertical < 0)  //brake
            {
                cart.AddForce(transform.forward * vertical * thrust  * driftBoost * weightPenalty * 0.6f);
            }
            if (horizontal != 0)  //rotation
            {
                cart.AddTorque(transform.up * horizontal * angular * weightPenalty * 1f);
            }

            driftV = driftValue();
            checkDrifting();
            checkTipping();
            addDriftScore(weightPenalty);

            //Debug.Log(driftScore +" | "+ driftBoost + " | " + tippingThreshold);

            //interpolating values
            tippingThreshold = tippingThreshold + (fixedTippingThreshold - tippingThreshold) * 0.005f;
            driftBoost = driftBoost + (1 - driftBoost) * 0.01f;
        }
        else
        {
            if (Input.GetKeyDown("r"))
            {
                cart.isKinematic = true;
                propUp = true;
                Debug.Log("R");
            }
            //TODO: detect being upright

            if (propUp)
            {
                makeUpright();
            }

            
        }
        
    }
    float driftValue()
    {
        return Mathf.Abs(Vector3.Dot(transform.right, cart.velocity) * ((1 + (weight / weightMax)) / 2));
    }

    void checkTipping()
    {
        if (driftV > tippingThreshold)
        {
            activateRagdoll();
        }
    }

    void checkDrifting()
    {
        if (!ragdoll && driftV > mindrift)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }
    }

    void activateNormal()
    {
        ragdoll = false;
        cart.isKinematic = false;
        GetComponent<BoxCollider>().material = slipperyM;
    }

    void activateRagdoll()
    {
        ragdoll = true;
        GetComponent<BoxCollider>().material = ragdollM;
    }

    void addDriftScore(float weightPenalty)
    {
        if (isDrifting)
        {
            driftScore += driftV * Time.deltaTime * 10 * weightPenalty;
            if (driftScore > minBoost)
            {
                boostReady = true;
            }
        }
        else
        {
            if (boostReady)
            {
                driftBoost += Mathf.Min(driftScore / 350, 1);
                tippingThreshold += Mathf.Min(driftScore / 150, 8);
            }
            driftScore = 0;
            boostReady = false;
        }

    }

    public bool getIsDrifting()
    {
        return isDrifting;
    }

    public bool getBoostReady()
    {
        return boostReady;
    }

    private void makeUpright() //raises the cart and changes its rotation to the last rotation before crashing
    {
        Debug.Log("doing stuff");
        transform.position = new Vector3(transform.position.x, Mathf.Min(transform.position.y + 0.02f, 2), transform.position.z);
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, 2, transform.position.z), ref vel, 0.5f);
        
        //Position = Position + (new Godot.Vector3(Position.X, 2, Position.Z) - Position) * (float)(10 * delta);
        //Rotation = Rotation + (lastOrientation - Rotation) * (float)(5 * delta);
    }
}
