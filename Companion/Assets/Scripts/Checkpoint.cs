using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public static Checkpoint current;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			current = this;
		}
	}
}
