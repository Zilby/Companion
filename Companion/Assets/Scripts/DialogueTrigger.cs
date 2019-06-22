using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public float initialDelay = 0;

	[System.Serializable]
	public struct DialogueSegment
	{
		public AudioClip clip;
		public string text;
		public float delay;
	}


	public List<DialogueSegment> segments;


	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			StartCoroutine(Dialogue());
			GetComponent<Collider>().enabled = false;
		}
	}


	public IEnumerator Dialogue()
	{
		yield return initialDelay;
		foreach (DialogueSegment d in segments)
		{
			UIManager.instance.DisplayDialogue(d.text);
			yield return LumpyController.instance.PlaySegment(d.clip);
			UIManager.instance.FadeDialogue();
			yield return new WaitForSeconds(d.delay);
		}
		yield return null;
		Destroy(gameObject);
	}
}
