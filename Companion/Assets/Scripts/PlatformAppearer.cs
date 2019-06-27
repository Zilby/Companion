using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PlatformAppearer : MonoBehaviour
{
	public Transform platformSet;

	public List<GameObject> platforms;

	public Material defaultM;
	public Material defaultP;
	public Material mMat;
	public Material pMat;

	[InspectorButton("ResetMaterials")]
	public bool SetUpMaterials;

	public void ResetMaterials()
	{
		defaultM = Resources.Load<Material>("Platform");
		mMat = new Material(defaultM);
		defaultP = Resources.Load<Material>("PlatformParticles");
		pMat = new Material(defaultP);
#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}

	void OnValidate()
	{
		if (platformSet != null)
		{
			platforms = new List<GameObject>();
			foreach (Transform child in platformSet)
			{
				platforms.Add(child.gameObject);
			}
			platformSet = null;
		}
	}

	private void Reset()
	{
		ResetMaterials();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			StartCoroutine(Appear());
		}
	}

	public IEnumerator Appear()
	{
		List<MeshRenderer> mRends = new List<MeshRenderer>();
		List<ParticleSystemRenderer> pRends = new List<ParticleSystemRenderer>();

		foreach (GameObject p in platforms)
		{
			p.SetActive(true);
			MeshRenderer[] ms = p.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer m in ms)
			{
				m.sharedMaterial = mMat;
				mRends.Add(m);
			}
			ParticleSystemRenderer[] ps = p.GetComponentsInChildren<ParticleSystemRenderer>();
			foreach (ParticleSystemRenderer pr in ps)
			{
				pr.sharedMaterial = pMat;
				pRends.Add(pr);
			}
		}
		Color mC = mMat.GetColor("_Color");
		Color mP = pMat.GetColor("_Color");
		mMat.SetColor("_Color", mC.A(0));
		pMat.SetColor("_Color", mP.A(0));
		float t = 0;
		while (true)
		{
			t = Mathf.Min(t, 1);
			mMat.SetColor("_Color", Color.Lerp(mC.A(0), mC, t));
			//pMat.SetColor("_Color", Color.Lerp(mP.A(0), mP, t));
			if (t >= 1)
			{
				break;
			}
			float duration = t > 230f / 255f ? 5f : (t > 200f / 255f ? 2f : 0.5f);
			t += Time.deltaTime / duration;
			yield return null;
		}

		for (int i = 0; i < platforms.Count; ++i)
		{
			if (mRends[i].sharedMaterial == mMat)
			{
				mRends[i].sharedMaterial = defaultM;
				pRends[i].sharedMaterial = defaultP;
			}
		}

		Destroy(gameObject);
		yield return null;
	}
}
