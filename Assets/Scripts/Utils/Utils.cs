﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General utility class
/// </summary>
public static class Utils
{
	/// <summary>
	/// Finds a child by name recursively. 
	/// </summary>
	public static T FindDeepChild<T>(Transform t, string name)
	{
		Transform tr = t.Find(name);
		T result = default(T);
		if (tr != null)
		{
			result = tr.GetComponent<T>();
		}
		if (result == null)
		{
			foreach (Transform child in t)
			{
				result = FindDeepChild<T>(child, name);
				if (result != null)
					break;
			}
		}
		return result;
	}

	/// <summary>
	/// Fades out an audio source. 
	/// </summary>
	public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
	{
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = startVolume;
	}

	/// <summary>
	/// Fades in an audio source. 
	/// </summary>
	public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
	{
		float startVolume = 0.2f;
		float originalVolume = audioSource.volume;
		audioSource.volume = 0;
		audioSource.Play();

		while (audioSource.volume < originalVolume)
		{
			audioSource.volume += startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.volume = originalVolume;
	}

	/// <summary>
	/// Moves an object to the given location. 
	/// </summary>
	public static IEnumerator MoveToLocation(Transform t, Vector2 location, float speed = 0.01f)
	{
		while (Mathf.Abs(location.x) > 0 || Mathf.Abs(location.y) > 0)
		{
			float xspeed = location.x * speed;
			float yspeed = location.y * speed;
			t.position = new Vector3(t.position.x + xspeed, t.position.y + yspeed, t.position.z);
			location.x -= xspeed;
			location.y -= yspeed;
			yield return new WaitForFixedUpdate();
		}
	}

	private static System.Random rng = new System.Random();

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
