using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
		LumpyController.instance.StartCoroutine(LumpyController.instance.Blink());
		LumpyController.instance.StartBobbing();
		// LumpyController.instance.spinning = true;
		float duration = 10f;
		while (Vector3.Distance(transform.parent.localPosition, Vector3.forward) > 0.01f)
		{
			float deltaTime = duration * (1.0f - Mathf.Exp(-Time.deltaTime));
			float smoothTime = Mathf.SmoothStep(0.0f, 1.0f, deltaTime);
			transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, Vector3.forward, smoothTime);
			transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, Quaternion.identity, smoothTime);
			yield return null;
		}
		yield return GameController.instance.eventBasedDialogues[0].Dialogue();

		Vector3 holdPosition = new Vector3(0.75f, -0.3f, 1f);
		Vector3 holdRotation = new Vector3(12.0f, 38.0f, 0f);
		while (Vector3.Distance(transform.parent.localPosition, holdPosition) > 0.01f)
		{
			float deltaTime = duration * (1.0f - Mathf.Exp(-Time.deltaTime));
			float smoothTime = Mathf.SmoothStep(0.0f, 1.0f, deltaTime);
			transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, holdPosition, smoothTime);
			transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, Quaternion.Euler(holdRotation), smoothTime);
			yield return null;
		}
		yield return null;
		StartCube.instance.StartCoroutine(StartCube.instance.FadeWall());
		Destroy(gameObject);
	}
}
