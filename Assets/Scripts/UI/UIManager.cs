using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public Fadeable darkener;
	public TextMeshProUGUI instructionText;
	public Fadeable instructionTextFadeable;
	public TextMeshProUGUI dialogueText;
	public Fadeable dialogueTextFadeable;

	private Coroutine checkForInput;
	private bool mouseMoved = false;
	private bool characterMoved = false;
	private bool characterJumped = false;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		darkener.Show();
		StartCoroutine(Tutorial());
		checkForInput = StartCoroutine(CheckForInput());
	}

	private IEnumerator CheckForInput()
	{
		for (; ; )
		{
			if (!mouseMoved && (
				Mathf.Abs(CrossPlatformInputManager.GetAxis("Mouse X") * FirstPersonController.main.m_MouseLook.XSensitivity) > 0 ||
				Mathf.Abs(CrossPlatformInputManager.GetAxis("Mouse Y") * FirstPersonController.main.m_MouseLook.YSensitivity) > 0))
			{
				mouseMoved = true;
			}

			if (!characterMoved && (
				Mathf.Abs(CrossPlatformInputManager.GetAxis("Horizontal")) > 0 ||
				Mathf.Abs(CrossPlatformInputManager.GetAxis("Vertical")) > 0))
			{
				characterMoved = true;
			}

			if (!characterJumped && CrossPlatformInputManager.GetButtonDown("Jump"))
			{
				characterJumped = true;
			}

			yield return null;
		}
	}

	private IEnumerator Tutorial()
	{
		yield return darkener.DelayedFadeOut(dur: 1);

		instructionText.text = "Use the mouse to look around";
		yield return new WaitForSeconds(0.5f);
		yield return instructionTextFadeable.FadeIn();
		yield return new WaitForSeconds(2f);
		while (!mouseMoved)
		{
			yield return new WaitForSeconds(0.25f);
		}
		yield return instructionTextFadeable.FadeOut();

		instructionText.text = "Use WASD to move";
		yield return new WaitForSeconds(0.25f);
		yield return instructionTextFadeable.FadeIn();
		yield return new WaitForSeconds(2f);
		while (!characterMoved)
		{
			yield return new WaitForSeconds(0.25f);
		}
		yield return instructionTextFadeable.FadeOut();

		instructionText.text = "Use the spacebar to jump";
		yield return new WaitForSeconds(0.25f);
		yield return instructionTextFadeable.FadeIn();
		yield return new WaitForSeconds(2f);
		while (!characterJumped)
		{
			yield return new WaitForSeconds(0.25f);
		}
		yield return instructionTextFadeable.FadeOut();

		StopCoroutine(checkForInput);
		StartCoroutine(GameController.instance.SpawnLumpy());
	}

	/// <summary>
	/// Displays the text.
	/// </summary>
	/// <param name="text">Text.</param>
	public void DisplayText(string text)
	{
		instructionText.text = text;
		instructionTextFadeable.SelfFadeIn();
	}

	/// <summary>
	/// Fades the text.
	/// </summary>
	public void FadeText()
	{
		instructionTextFadeable.SelfFadeOut();
	}


	/// <summary>
	/// Displays the dialogue.
	/// </summary>
	/// <param name="text">Text.</param>
	public IEnumerator DisplayDialogue(string text)
	{
		dialogueText.text = "";
		dialogueTextFadeable.SelfFadeIn(dur:0.2f);

		string colorCode = "<color=#00000000>";
		string colorTerminator = "</color>";
		
		foreach (char letter in text)
		{
			if (letter == '_')
			{
				continue; 
			}

			dialogueText.text += colorCode + letter + colorTerminator;
		}

		const string letters = "abcdefghijklmnopqrstuvwxyz";

		int index = 0; 
		int skipLetter = 0;
		foreach (char letter in text)
		{
			if (letter == '_')
			{
				yield return new WaitForSecondsRealtime(0.2f);
				continue;
			}

			index += colorCode.Length;

			char[] chars = dialogueText.text.ToCharArray();
			chars[index - 2] = 'F';
			chars[index - 3] = 'F';
			dialogueText.text = new string(chars);

			index += 1 + colorTerminator.Length;


			int alphabetIndex = letters.IndexOf(Char.ToLower(letter));
			if (alphabetIndex >= 0)
			{
				if (skipLetter % 2 == 0) 
				{
					LumpyController.instance.PlaySegment(alphabetIndex);
				}
				if (skipLetter % 4 == 0)
				{
					StartCoroutine(LumpyController.instance.MoveMouth());
				}
				skipLetter++;
			}

			if (".?!".Contains(letter))
			{
				yield return new WaitForSecondsRealtime(0.3f);
			}
			else if (letter == ' ')
			{
				yield return new WaitForSecondsRealtime(0.05f);
			}
			else
			{
				yield return new WaitForSecondsRealtime(0.03f);
			}
		}
	}

	/// <summary>
	/// Fades the dialogue.
	/// </summary>
	public void FadeDialogue()
	{
		dialogueTextFadeable.SelfFadeOut(dur:0.2f);
	}


	/// <summary>
	/// Fades the screen black and back. 
	/// </summary>
	/// <param name="fadeDur">Fade dur.</param>
	/// <param name="delay">Delay.</param>
	public IEnumerator FadeInNOut(float fadeDur, float delay, Func<IEnumerator> f)
	{
		yield return darkener.FadeIn(dur: fadeDur);
		yield return f();
		yield return new WaitForSeconds(delay);
		yield return darkener.FadeOut(dur: fadeDur);
	}
}
