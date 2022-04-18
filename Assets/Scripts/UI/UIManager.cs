using System;
using System.Collections;
using System.Collections.Generic;
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
	public void DisplayDialogue(string text)
	{
		dialogueText.text = text;
		dialogueTextFadeable.SelfFadeIn(dur:0.2f);
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
