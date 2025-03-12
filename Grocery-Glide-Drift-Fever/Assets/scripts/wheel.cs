using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheel : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private ParticleSystem smoke, sparks;
	private Cart cart;


    private void Start()
    {
        cart = rb.GetComponent<Cart>();
		smoke.Stop();
		sparks.Stop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (new Vector3(rb.velocity.x,0,rb.velocity.z).magnitude > 0)
		{
			transform.LookAt(transform.position+rb.velocity);
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
		}

		if (cart.GetIsDrifting()) 
		{
			smoke.Play();
		}
		else
		{
			smoke.Stop();
		}

		Debug.Log(cart.GetBoostReady());
		if (cart.GetBoostReady()) {
			sparks.Play();
		}
		else
		{
			sparks.Pause();
		}
    }
}

/* 
public partial class Wheel : MeshInstance3D
{

	private Cart cart;
	private Vector3 velocityDir, velocity2D;
	private GpuParticles3D smoke, sparks;

	public float driftAngleThresh = 0.7f;
	public float driftVelocityThresh = 7f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cart = GetParent<Cart>();
		smoke = GetChild<GpuParticles3D>(0);
		sparks = GetChild<GpuParticles3D>(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		velocity2D = new Vector3(cart.LinearVelocity.X, 0, cart.LinearVelocity.Z);
		velocityDir = GlobalPosition + velocity2D;
		LookInVelocityDirection();


		if (cart.isDrifting)
		{
			smoke.Emitting = true;
		}
		else
		{
			smoke.Emitting = false;
		}
		if(cart.boostReady)
		{
			sparks.Emitting =true;
		}
		else
		{
			sparks.Emitting =false;
		}

	}

	private void LookInVelocityDirection()
	{
		if (velocity2D.Length() > 0.1)
		{
			LookAt(velocityDir);
			Rotation = new Vector3(0, Rotation.Y, 0);
		}
	}
}
*/

