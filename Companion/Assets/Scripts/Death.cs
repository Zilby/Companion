using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using System;

public class Death : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			FirstPersonController.main.dying = true;
			StartCoroutine(UIManager.instance.FadeInNOut(0.4f, 0.4f, RepositionPlayer));
		}
	}

	IEnumerator RepositionPlayer() {
		FirstPersonController.main.enabled = false;
		FirstPersonController.main.transform.position = Checkpoint.current.transform.position;
		yield return null;
		FirstPersonController.main.enabled = true;
	}
}
