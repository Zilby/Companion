using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PlatformAppearer : MonoBehaviour
{
	public static List<PlatformAppearer> platformAppearers = new List<PlatformAppearer>();
	public Transform platformSet;

	public List<GameObject> platforms;

	private bool bHidStartCube = false;
	private bool bAppeared = false;

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

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (platforms.Count > 0 && !bAppeared)
			{
				Appear();
				platformAppearers.Add(this);
				if (platformAppearers.Count > 2 && !bHidStartCube)
				{
					StartCube.instance.Disappear();
					bHidStartCube = true;
				}
				if (platformAppearers.Count > 3)
				{
					platformAppearers[0].Disappear();
					platformAppearers.RemoveAt(0);
				}
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	public void Appear()
	{
		foreach (GameObject p in platforms)
		{
			p.SetActive(true);
			Platform plat = p.GetComponent<Platform>();
			plat.StartCoroutine(plat.FadeIn(dur: 0.8f));
		}
		bAppeared = true;
	}

	public void Disappear()
	{
		foreach (GameObject p in platforms)
		{
			p.SetActive(true);
			Platform plat = p.GetComponent<Platform>();
			plat.StartCoroutine(plat.FadeOut(dur: 0.8f));
		}
	}
}
