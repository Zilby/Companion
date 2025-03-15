using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Platform class for managing platform behavior. 
/// </summary>
public class Platform : FadeableMesh
{
	public bool sticky = false;

	public Material defaultM;

	public Material defaultMPS;

	public Material friendlyM;

	public Material friendlyMPS;

	private ParticleSystemRenderer[] ps;

	private MeshRenderer[] rends;

	private AudioSource aSource;

	public AudioClip clip;

	private Color defaultColor;

	private Color friendlyColor;

	private Color friendlyMPSColor;

	private Material mInstance;

	private Material mPSInstance;

	private Coroutine colorLerp;

	protected override void Reset()
	{
		base.Reset();
		defaultM = Resources.Load<Material>("Platform");
		defaultMPS = Resources.Load<Material>("PlatformParticles");
		friendlyM = Resources.Load<Material>("PlatformFriendly");
		friendlyMPS = Resources.Load<Material>("PlatformParticlesFriendly");
		clip = Resources.Load<AudioClip>("Basic Note Low");
		
#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}

    protected override void Awake()
    {
		base.Awake();
		ps = GetComponentsInChildren<ParticleSystemRenderer>();
		rends = GetComponentsInChildren<MeshRenderer>();
		aSource = GetComponent<AudioSource>();

		defaultColor = defaultM.GetColor("_EmissionColor");
		friendlyColor = friendlyM.GetColor("_EmissionColor");
		friendlyMPSColor = friendlyMPS.GetColor("_EmissionColor");

        mInstance = new Material(defaultM);
		mPSInstance = new Material(defaultMPS);

		foreach (MeshRenderer rend in rends)
		{
			rend.sharedMaterial = mInstance;
		}
		foreach (ParticleSystemRenderer pr in ps)
		{
			pr.sharedMaterial = mPSInstance;
		}
    }


    public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			FirstPersonController.main.sticky = sticky;
			FirstPersonController.main.m_StickToGroundForce = sticky ? 10 : 1;
			
			if (colorLerp != null)
			{
				StopCoroutine(colorLerp);
			}
			aSource.pitch = Random.Range(0.45f, 1.3f);
			aSource.PlayOneShot(clip);
			SetFriendlyColors();
		}
	}


	public void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			FirstPersonController.main.sticky = false;
			FirstPersonController.main.m_StickToGroundForce = 1;
			if (colorLerp != null)
			{
				StopCoroutine(colorLerp);
			}
			colorLerp = StartCoroutine(ColorLerp(0.6f, friendlyColor, defaultColor, friendlyMPSColor, defaultColor));
		}
	}


	/// <summary>
	/// Sets platform to the default friendly colors.
	/// </summary>
	private void SetFriendlyColors()
	{
		mInstance.SetColor("_EmissionColor", friendlyColor);
		mPSInstance.SetColor("_EmissionColor", friendlyMPSColor);
		mPSInstance.SetColor("_Color", friendlyColor);
	}

	private IEnumerator ColorLerp(float duration, Color mColor1, Color mColor2, Color mPSColor1, Color mPSColor2)
	{
		float t = 0;
		while (true)
		{
			t = Mathf.Min(t, 1);
			Color CurrentMColor = Color.Lerp(mColor1, mColor2, t);
			Color CurrentMPSColor = Color.Lerp(mPSColor1, mPSColor2, t);
			mInstance.SetColor("_EmissionColor", CurrentMColor);
			mPSInstance.SetColor("_EmissionColor", CurrentMPSColor);
			mPSInstance.SetColor("_Color", CurrentMColor); // This is intentionally the platform color so it's more muted
			if (t >= 1)
			{
				break;
			}
			t += Time.deltaTime / duration;
			yield return null;
		}
	}
}
