using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
	public static GameController instance;

	public GameObject lumpy;

	public List<DialogueTrigger> eventBasedDialogues;
	
	[Range(0, 2)]
	public float timescale = 1;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		Time.timeScale = timescale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
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
