using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public float initialDelay = 0;

	[System.Serializable]
	public struct DialogueSegment
	{
		public LumpyController.Expression expression;
		public string text;
		public float delay;

		public List<DialogueResponse> responses;
	}

	[System.Serializable]
	public struct DialogueResponse
	{
		public LumpyController.Expression expression;
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
			LumpyController.instance.StartCoroutine(LumpyController.instance.SetExpression(d.expression));
			yield return UIManager.instance.DisplayDialogue(d.text);
			yield return new WaitForSeconds(d.delay);

			if (d.responses.Count > 2)
			{
				yield return UIManager.instance.DisplayQuestion(d.responses);
			}
			yield return UIManager.instance.dialogueTextFadeable.FadeOut(dur:0.2f);
		}
		yield return null;
		Destroy(gameObject);
	}
}
