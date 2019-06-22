using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Platform class for managing platform behavior. 
/// </summary>
public class Platform : MonoBehaviour
{
	public Material defaultM;

	public Material friendlyM;

	public Material defaultMPS;

	public Material friendlyMPS;

	public ParticleSystemRenderer ps;

	public MeshRenderer rend;

	public Color defaultColor;

	public Color friendlyColor;

	public AudioSource aSource;

	public AudioClip clip;

	private Coroutine colorLerp;


	private void Reset()
	{
		ps = GetComponentInChildren<ParticleSystemRenderer>();
		rend = GetComponent<MeshRenderer>();
		aSource = GetComponent<AudioSource>();
		defaultM = Resources.Load<Material>("Platform");
		friendlyM = new Material(defaultM);
		defaultMPS = Resources.Load<Material>("PlatformParticles");
		friendlyMPS = new Material(defaultMPS);
		defaultColor = defaultM.GetColor("_EmissionColor");
		friendlyColor = Resources.Load<Material>("PlatformFriendly").GetColor("_EmissionColor");
		SetFriendlyColors();

		clip = Resources.Load<AudioClip>("Basic Note Low");
	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			rend.sharedMaterial = friendlyM;
			ps.sharedMaterial = friendlyMPS;
			if (colorLerp != null)
			{
				StopCoroutine(colorLerp);
			}
			aSource.pitch = Random.Range(0.3f, 1.3f);
			aSource.PlayOneShot(clip);
			SetFriendlyColors();
			//colorLerp = StartCoroutine(ColorLerp(0.05f, defaultColor, friendlyColor, friendlyM, friendlyMPS));
		}
	}


	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			if (colorLerp != null)
			{
				StopCoroutine(colorLerp);
			}
			colorLerp = StartCoroutine(ColorLerp(0.6f, friendlyColor, defaultColor, defaultM, defaultMPS));
		}
	}


	/// <summary>
	/// Sets platform to the default friendly colors.
	/// </summary>
	private void SetFriendlyColors()
	{
		friendlyM.SetColor("_EmissionColor", friendlyColor);
		friendlyMPS.SetColor("_EmissionColor", friendlyColor);
		friendlyMPS.SetColor("_Color", friendlyColor);
	}


	/// <summary>
	/// Lerps the material colors
	/// </summary>
	/// <param name="duration">Duration.</param>
	/// <param name="color1">Color1.</param>
	/// <param name="color2">Color2.</param>
	/// <param name="mat1">Platform mat</param>
	/// <param name="mat2">Particle System mat</param>
	private IEnumerator ColorLerp(float duration, Color color1, Color color2, Material mat1, Material mat2)
	{
		float t = 0;
		while (true)
		{
			t = Mathf.Min(t, 1);
			friendlyM.SetColor("_EmissionColor", Color.Lerp(color1, color2, t));
			if (t >= 1)
			{
				break;
			}
			t += Time.deltaTime / duration;
			yield return null;
		}
		rend.sharedMaterial = mat1;
		ps.sharedMaterial = mat2;
	}
}
