using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpLumpy : MonoBehaviour
{
	private bool pickingUp = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !pickingUp)
		{
			UIManager.instance.DisplayText("Press F to Pick Up");
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player" && Input.GetKeyDown(KeyCode.F) && !pickingUp)
		{
			pickingUp = true;
			UIManager.instance.FadeText();
			StartCoroutine(PickUp());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" && !pickingUp)
		{
			UIManager.instance.FadeText();
		}
	}

	private IEnumerator PickUp()
	{
		transform.parent.parent = Camera.main.transform;
		Rigidbody rBody = GetComponentInParent<Rigidbody>();
		rBody.isKinematic = true;
		LumpyController.instance.spinning = true;
		float smoothTime = 0.5f;
		Vector3 velocity = Vector3.zero;
		while (Vector3.Distance(transform.parent.localPosition, Vector3.forward) > 0.01f)
		{
			transform.parent.localPosition = Vector3.SmoothDamp(transform.parent.localPosition, Vector3.forward, ref velocity, smoothTime);
			yield return null;
		}
		yield return GameController.instance.eventBasedDialogues[0].Dialogue();
		velocity = Vector3.zero;
		Vector3 holdPosition = new Vector3(0.65f, -0.36f, 1f);
		while (Vector3.Distance(transform.parent.localPosition, holdPosition) > 0.01f)
		{
			transform.parent.localPosition = Vector3.SmoothDamp(transform.parent.localPosition, holdPosition, ref velocity, smoothTime);
			yield return null;
		}
		yield return null;
		GameController.instance.StartCoroutine(GameController.instance.FadeWall());
		Destroy(gameObject);
	}
}
