using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelBehaviour : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	private CartMovement _cartMovement;
	[SerializeField] private ParticleSystem smoke, sparks;
	[SerializeField] private TrailRenderer streaks;
	private void Start()
	{
		_cartMovement = rb.GetComponent<CartMovement>();
		smoke.Stop();
		sparks.Stop();
		sparks.Clear();
	}
	private void FixedUpdate()
	{
		if (new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 0)
		{
			transform.LookAt(transform.position + rb.velocity);
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		}
	}

	public bool IsGrounded()
	{
		return Physics.Raycast(transform.position, -transform.up, 0.14f, LayerMask.GetMask("Environment"));
	}

	/// <summary>
	/// Checks if the Wheel is grounded and if so, plays smoke particles
	/// </summary>
	public void PlaySmoke()
	{
		if (IsGrounded())
		{
			smoke.Play();
			streaks.emitting = true;
		}
		else
		{
			streaks.emitting = false;
		}
	}

	public void StopSmoke()
	{
		smoke.Stop();
		streaks.emitting = false;
	}

	
	/// <summary>
	/// Checks if the Wheel is grounded and if so, plays spark particles
	/// </summary>
	public void PlaySpark()
	{
		if (IsGrounded())
		{
			sparks.Play();
		}
	}

	public void StopSpark()
	{
		sparks.Stop();
		sparks.Clear();
	}
}