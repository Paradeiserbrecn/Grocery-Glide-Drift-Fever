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

	// Update is called once per frame
	private void FixedUpdate()
	{
		if (new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 0)
		{
			transform.LookAt(transform.position + rb.velocity);
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		}
		
		if (_cartMovement.BoostReady)
		{
			sparks.Play();
		}
		else
		{
			sparks.Stop();
			sparks.Clear();
		}

		if (_cartMovement.IsDrifting)
		{
			smoke.Play();
		}
		else
		{
			smoke.Stop();
		}
	}

	/// <summary>
	/// Checks if the Wheel is grounded and if so, plays smoke particles
	/// </summary>
	public void PlaySmoke()
	{
		if (Physics.Raycast(transform.position, -transform.up, 0.1f))
		{
			Debug.Log("hello?");
			smoke.Play();
			streaks.widthMultiplier = 1f;
		}
	}

	public void StopSmoke()
	{
		smoke.Stop();
		streaks.widthMultiplier = 0f;
	}

	
	/// <summary>
	/// Checks if the Wheel is grounded and if so, plays spark particles
	/// </summary>
	public void PlaySpark()
	{
		if (Physics.Raycast(transform.position, -transform.up, 0.05f))
		{
			sparks.Play();
		}
	}

	public void StopSpark()
	{
		sparks.Stop();
	}
}