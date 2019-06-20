using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameController : MonoBehaviour
{
	public GameObject lumpy;

	public static GameController instance;

	private void Awake()
	{
		instance = this;
	}

	public IEnumerator SpawnLumpy()
	{
		while (Mathf.Abs(FirstPersonController.main.transform.eulerAngles.y) < 140 ||
			   Mathf.Abs(FirstPersonController.main.transform.eulerAngles.y) > 220)
		{
			yield return null;
		}
		lumpy.SetActive(true);
	}

}
