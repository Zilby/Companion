using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumpyController : MonoBehaviour
{
	public static LumpyController instance;

	[System.NonSerialized]
	public bool spinning = false;


	private void Awake()
	{
		instance = this;
	}

	void Update()
	{
		if (spinning)
		{
			transform.Rotate(Vector3.forward, 0.5f);
			transform.Rotate(Vector3.up, 3f);
			transform.Rotate(Vector3.right, 0.3f);
		}
	}
}
