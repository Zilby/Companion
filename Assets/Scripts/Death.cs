using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using System;

public class Death : MonoBehaviour
{
	public float maxVelocity = -60.0f;
	private float delay = 0;

    private void FixedUpdate()
    {
        if (FirstPersonController.main.Velocity.y < maxVelocity & delay <= 0)
		{
			delay = 5;
			Kill();
		}
		delay -= Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Kill();
		}
	}

	private void Kill()
	{
		FirstPersonController.main.dying = true;
		StartCoroutine(UIManager.instance.FadeInNOut(0.4f, 0.4f, RepositionPlayer));
	}

	IEnumerator RepositionPlayer()
	{
		FirstPersonController.main.enabled = false;
		FirstPersonController.main.transform.position = Checkpoint.current.transform.position;
		yield return null;
		FirstPersonController.main.enabled = true;
	}
}
