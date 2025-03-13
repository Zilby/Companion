using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LumpyController : MonoBehaviour
{

	public enum Expression
	{
		Neutral,
		Surprised,
		Happy, 
		Delighted, 
		Annoyed, 
		Angry, 
		Sad,
		Unimpressed, 
		Tired, 
		Asleep, 
		Inquisitive,
	}

	private Color DEFAULT_COLOR = new Color(5f / 255f, 191 / 255f, 183 / 255f);  // 05BFB7 new Color(0, 134 / 255f, 191 / 255f); 
	private const float INTENSITY = 1f; // 2.1f; //1.188807F;

	public static LumpyController instance;

	public SkinnedMeshRenderer eyes;

	public SkinnedMeshRenderer mouth;

	public List<AudioClip> dialogueAudioClips;

	[System.NonSerialized]

	private Expression currentExpression = Expression.Neutral;

	private Coroutine talk;

	private bool bMovingMouth;

	private AudioSource aSource;

	private void Awake()
	{
		instance = this;
		aSource = GetComponent<AudioSource>();
	}

	public void PlaySegment(int index) 
	{
		aSource.clip = dialogueAudioClips[index];
		aSource.Play();
	}

	public IEnumerator SetExpression(Expression expression)
	{
		if (currentExpression == expression)
		{
			yield break;
		}
		this.currentExpression = expression;
		float duration = 0.1f;
		float timeElapsed = 0;
		Dictionary<int, float> oldEyeWeights = new Dictionary<int, float>();
		Dictionary<int, float> oldMouthWeights = new Dictionary<int, float>();
		List<(Dictionary<int, float>, SkinnedMeshRenderer)> Weights = new List<(Dictionary<int, float>, SkinnedMeshRenderer)>
		{
			(oldEyeWeights, eyes),
			(oldMouthWeights, mouth)
		};
		
		foreach (var Pair in Weights)
		{
			// first index is for talking / blinking
			for (int i = 1; i < Pair.Item2.sharedMesh.blendShapeCount; ++i)
			{
				Pair.Item1.Add(i, eyes.GetBlendShapeWeight(i));
			}
		}

		Dictionary<int, float> newEyeWeights = new Dictionary<int, float>();
		Dictionary<int, float> newMouthWeights = new Dictionary<int, float>();
		switch (currentExpression)
		{
			case Expression.Neutral:
				break;
			case Expression.Surprised:
				newEyeWeights.Add(1, 100);
				break;
			case Expression.Happy:
				newEyeWeights.Add(2, 100);
				newMouthWeights.Add(1, 100);
				break;
			case Expression.Delighted: 
				newEyeWeights.Add(3, 100);
				newMouthWeights.Add(1, 100);
				newMouthWeights.Add(2, 100);
				break;
			case Expression.Annoyed:
				newEyeWeights.Add(4, 100);
				newMouthWeights.Add(3, 50);
				break;
			case Expression.Angry:
				newEyeWeights.Add(5, 100);
				newMouthWeights.Add(3, 40);
				newMouthWeights.Add(4, 25);
				break;
			case Expression.Sad:
				newEyeWeights.Add(6, 100);
				newMouthWeights.Add(4, 100);
				break;
			case Expression.Unimpressed:
				newEyeWeights.Add(7, 100);
				newMouthWeights.Add(3, 25);
				break;
			case Expression.Tired:
				newEyeWeights.Add(8, 100);
				newMouthWeights.Add(3, 25);
				break;
			case Expression.Asleep:
				newEyeWeights.Add(9, 100);
				newMouthWeights.Add(3, 100);
				break;
			case Expression.Inquisitive:
				newEyeWeights.Add(10, 100);
				break;
		}
		while (timeElapsed < duration)
        {
			for (int i = 1; i < eyes.sharedMesh.blendShapeCount; ++i)
			{
				float newWeight = 0;
				if (newEyeWeights.ContainsKey(i))
				{
					newWeight = newEyeWeights[i];
				}
				eyes.SetBlendShapeWeight(i, Mathf.Lerp(oldEyeWeights[i], newWeight, timeElapsed / duration));
			}

			for (int i = 1; i < mouth.sharedMesh.blendShapeCount; ++i)
			{
				float newWeight = 0;
				if (newMouthWeights.ContainsKey(i))
				{
					newWeight = newMouthWeights[i];
				}
				mouth.SetBlendShapeWeight(i, Mathf.Lerp(oldMouthWeights[i], newWeight, timeElapsed / duration));
			}
            
            yield return null;
            timeElapsed += Time.deltaTime;
        }
	}

	public IEnumerator Blink()
	{
		yield return new WaitForSecondsRealtime(Random.Range(2f, 4.5f));
		float duration = 0.05f;

		float timeElapsed = 0;
		while (timeElapsed < duration)
        {
            eyes.SetBlendShapeWeight(0, Mathf.Lerp(0, GetBlinkMax(), timeElapsed / duration));
            yield return null;
            timeElapsed += Time.deltaTime;
        }

		timeElapsed = 0;
		while (timeElapsed < duration)
        {
            eyes.SetBlendShapeWeight(0, Mathf.Lerp(GetBlinkMax(), 0, timeElapsed / duration));
            yield return null;
            timeElapsed += Time.deltaTime;
        }

		eyes.SetBlendShapeWeight(0, 0);

		yield return Blink();
	}

	public float GetBlinkMax()
	{
		switch (currentExpression)
		{
			case Expression.Asleep:
				return 0;
			case Expression.Delighted:
				return 40;
			case Expression.Happy:
			case Expression.Sad:
			case Expression.Unimpressed:
				return 60;
			case Expression.Angry:
				return 70;
			case Expression.Annoyed:
			case Expression.Tired:
			case Expression.Inquisitive:
				return 80;
		}
		return 100;
	}

	public IEnumerator MoveMouth()
	{
		bMovingMouth = true;
		float inDuration = Random.Range(0.08f, 0.12f);
		float outDuration = Random.Range(0.08f, 0.12f);
		float timeElapsed = 0;
		while (timeElapsed < inDuration)
        {
            mouth.SetBlendShapeWeight(0, Mathf.SmoothStep(0, GetMouthMax(), timeElapsed / inDuration));
            yield return null;
            timeElapsed += Time.deltaTime;
        }

		yield return new WaitForSeconds(0.01f);

		timeElapsed = 0;
		while (timeElapsed < outDuration)
        {
            mouth.SetBlendShapeWeight(0, Mathf.SmoothStep(GetMouthMax(), 0, timeElapsed / outDuration));
            yield return null;
            timeElapsed += Time.deltaTime;
        }
		bMovingMouth = false;

		mouth.SetBlendShapeWeight(0, 0);
	}

	public float GetMouthMax()
	{
		switch (currentExpression)
		{
			case Expression.Asleep:
			case Expression.Delighted:
			case Expression.Happy:
			case Expression.Sad:
			case Expression.Unimpressed:
			case Expression.Angry:
			case Expression.Annoyed:
			case Expression.Tired:
			case Expression.Inquisitive:
				return 100;
		}
		return 100;
	}
}
