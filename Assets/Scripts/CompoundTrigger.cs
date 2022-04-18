using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompoundTrigger : MonoBehaviour
{
	public static Dictionary<Platform, int> triggerDict = new Dictionary<Platform, int>();

	private Platform key;

	private void Awake()
	{
		key = GetComponentInParent<Platform>();
		if (!triggerDict.ContainsKey(key))
		{
			triggerDict.Add(key, 0);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (triggerDict[key] == 0)
			{
				key.OnTriggerEnter(other);
			}
			triggerDict[key]++;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			if (triggerDict[key] == 1)
			{
				key.OnTriggerExit(other);
			}
			triggerDict[key]--;
		}
	}
}
