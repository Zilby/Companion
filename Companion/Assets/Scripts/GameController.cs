﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GameController : MonoBehaviour
{
	public static GameController instance;

	public GameObject lumpy;

	public FadeableMesh wall;

	[Range(0, 2)]
	public float timescale = 1;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		wall.Show();
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

	public IEnumerator FadeWall()
	{
		while (Mathf.Abs(FirstPersonController.main.transform.eulerAngles.y) < 140 ||
			   Mathf.Abs(FirstPersonController.main.transform.eulerAngles.y) > 220)
		{
			yield return null;
		}
		yield return wall.FadeOut(dur: 0.8f);
	}
}