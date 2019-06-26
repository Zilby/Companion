using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumpyController : MonoBehaviour
{
	private Color DEFAULT_COLOR = new Color(5f / 255f, 191 / 255f, 183 / 255f);  // 05BFB7 new Color(0, 134 / 255f, 191 / 255f); 
	private const float INTENSITY = 1f; // 2.1f; //1.188807F;

	public static LumpyController instance;

	[System.NonSerialized]
	public bool spinning = false;

	private Coroutine talk;

	private Renderer[] rends;

	private Material mat;

	private AudioSource aSource;

	private void Awake()
	{
		instance = this;
		rends = GetComponentsInChildren<Renderer>();
		mat = rends[0].sharedMaterial;
		mat.EnableKeyword("_EMISSION");
		mat.SetColor("_EmissionColor", DEFAULT_COLOR * INTENSITY);
		aSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (spinning)
		{
			transform.Rotate(Vector3.forward, 15f * Time.deltaTime);
			transform.Rotate(Vector3.up, 90f * Time.deltaTime);
			transform.Rotate(Vector3.right, 9f * Time.deltaTime);
		}
	}

	public void StartTalking()
	{
		talk = StartCoroutine(Talk());
	}

	public IEnumerator StopTalking()
	{
		StopCoroutine(talk);
		float t = 0;
		for (; ; )
		{
			t = Mathf.Min(t, 1);
			mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), DEFAULT_COLOR * INTENSITY, t));
			if (t >= 1)
			{
				break;
			}
			t += Time.deltaTime / 0.3f;
			yield return null;
		}
	}

	private IEnumerator Talk()
	{
		for (; ; )
		{
			//yield return new WaitForSeconds(Random.Range(0.04f, 0.12f));
			float r = Random.Range(0f, 90f); // Random.Range(15f, 140f);
			Color newColor = new Color(r / 255f, Random.Range(0, 190f) / 255f, Random.Range(100f + r, 190f) / 255f); 
						   //new Color(r / 255f, Random.Range(r, 190f) / 255f, Random.Range(170f, 220f) / 255f) * Random.Range(1.2f, 2.8f);
			float lerpTime = Random.Range(0.1f, 0.3f);
			//mat.SetColor("_EmissionColor", newColor);
			float t = 0;
			for (; ; )
			{
				t = Mathf.Min(t, 1);
				mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), newColor, t));
				if (t >= 1)
				{
					break;
				}
				t += Time.deltaTime / lerpTime;
				yield return null;
			}
		}
	}

	public IEnumerator PlaySegment(AudioClip clip) {
		aSource.clip = clip;
		aSource.Play();
		StartTalking();
		while(aSource.isPlaying) {
			yield return null;
		}
		yield return StopTalking();
	}
}
