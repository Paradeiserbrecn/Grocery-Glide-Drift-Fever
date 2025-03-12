using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelBehaviour : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private ParticleSystem smoke, sparks;
	private CartMovement _cartMovement;
	
    private void Start()
    {
        _cartMovement = rb.GetComponent<CartMovement>();
		smoke.Stop();
		sparks.Stop();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (new Vector3(rb.velocity.x,0,rb.velocity.z).magnitude > 0)
		{
			transform.LookAt(transform.position+rb.velocity);
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		}

		if (_cartMovement.IsDrifting) 
		{
			smoke.Play();
		}
		else
		{
			smoke.Stop();
		}

		if (_cartMovement.BoostReady) {
			sparks.Play();
		}
		else
		{
			sparks.Pause();
		}
    }
}