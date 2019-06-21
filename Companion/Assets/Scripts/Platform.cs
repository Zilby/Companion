using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private Coroutine colorLerp;

	private void Reset()
	{
		ps = GetComponentInChildren<ParticleSystemRenderer>();
		rend = GetComponent<MeshRenderer>();
		defaultM = Resources.Load<Material>("Platform");
		friendlyM = Resources.Load<Material>("PlatformFriendly");
		defaultMPS = Resources.Load<Material>("PlatformParticles");
		friendlyMPS = Resources.Load<Material>("PlatformParticlesFriendly");
		defaultColor = defaultM.GetColor("_EmissionColor");
		friendlyColor = friendlyM.GetColor("_EmissionColor");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			rend.sharedMaterial = friendlyM;
			ps.sharedMaterial = friendlyMPS;
			if (colorLerp != null) {
				StopCoroutine(colorLerp);
			}
			colorLerp = StartCoroutine(ColorLerp(defaultColor, friendlyColor));
		}
	}


	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			rend.sharedMaterial = defaultM;
			ps.sharedMaterial = defaultMPS;
			if (colorLerp != null)
			{
				StopCoroutine(colorLerp);
			}
			colorLerp = StartCoroutine(ColorLerp(friendlyColor, defaultColor));
		}
	}

	private IEnumerator ColorLerp(Color color1, Color color2)
	{
		float t = 0;
		float time = Time.time;
		while (Time.time - time < 0.15f)
		{
			friendlyM.SetColor("_EmissionColor", Color.Lerp(color1, color2, t));
			t += 7f * Time.deltaTime;
			yield return null;
		}
	}
}
