using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public Fadeable darkener;
	public TextMeshProUGUI instructionText;
	public Fadeable instructionTextFadeable;
	public GameObject dialogueContainer;
	public TextMeshProUGUI dialogueText;
	public Fadeable dialogueTextFadeable;
	public Fadeable choiceTextFadeable;
	public Image choiceBar;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		darkener.Show();
		StartCoroutine(Tutorial());
	}

	public void RefreshLayout(RectTransform transform) 
	{
		for (int i = 0; i < transform.childCount; ++i) 
		{
			var child = transform.GetChild(i);

			if (child is RectTransform rect) 
			{
				RefreshLayout(rect);
			}
		}

		if (transform.TryGetComponent<LayoutGroup>(out var layoutGroup)) 
		{
			layoutGroup.CalculateLayoutInputHorizontal();
			layoutGroup.CalculateLayoutInputVertical();
			layoutGroup.SetLayoutHorizontal();
			layoutGroup.SetLayoutVertical();
		}

		if (transform.TryGetComponent<ContentSizeFitter>(out var contentSizeFitter)) 
		{
			contentSizeFitter.SetLayoutHorizontal();
			contentSizeFitter.SetLayoutVertical();
		}
		// LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
    }		

	private IEnumerator Tutorial()
	{
		var textTransform = instructionTextFadeable.transform as RectTransform;
		yield return darkener.DelayedFadeOut(dur: 1);

		instructionText.text = "Use the mouse to look around";
		RefreshLayout(textTransform);
		yield return new WaitForSeconds(0.5f);
		instructionTextFadeable.SelfFadeIn();
		for (;;)
		{
			if (Mathf.Abs(CrossPlatformInputManager.GetAxis("Mouse X") * FirstPersonController.main.m_MouseLook.XSensitivity) > 0 ||
				Mathf.Abs(CrossPlatformInputManager.GetAxis("Mouse Y") * FirstPersonController.main.m_MouseLook.YSensitivity) > 0)
			{
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		yield return instructionTextFadeable.FadeOut();

		instructionText.text = "Use WASD to move";
		RefreshLayout(textTransform);
		yield return new WaitForSeconds(0.25f);
		instructionTextFadeable.SelfFadeIn();
		for (;;)
		{
			if (Mathf.Abs(CrossPlatformInputManager.GetAxis("Horizontal")) > 0 ||
				Mathf.Abs(CrossPlatformInputManager.GetAxis("Vertical")) > 0)
			{
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		yield return instructionTextFadeable.FadeOut();

		instructionText.text = "Use the spacebar to jump";
		RefreshLayout(textTransform);
		yield return new WaitForSeconds(0.25f);
		instructionTextFadeable.SelfFadeIn();
		for (;;)
		{
			if (CrossPlatformInputManager.GetButtonDown("Jump"))
			{
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		yield return instructionTextFadeable.FadeOut();

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
		RefreshLayout(instructionTextFadeable.transform as RectTransform);
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

		string colorCode = "<color=#FFFFFF00>";
		string colorTerminator = "</color>";
		
		foreach (char letter in text)
		{
			if (letter == '_')
			{
				continue; 
			}

			dialogueText.text += colorCode + letter + colorTerminator;
		}

		dialogueTextFadeable.SelfFadeIn(dur:0.2f);
		RefreshLayout(dialogueContainer.transform as RectTransform);

		const string letters = "abcdefghijklmnopqrstuvwxyz";
		// const string punctuation = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

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
				LumpyController.instance.PlaySegment(alphabetIndex);

				if (skipLetter % LumpyController.instance.mouthMoveFrequency == 0)
				{
					StartCoroutine(LumpyController.instance.MoveMouth());
				}
				skipLetter++;
			}

			if ("!?.".Contains(letter))
			{
				yield return new WaitForSecondsRealtime(0.6f);
			}
			else if (",;:".Contains(letter))
			{
				yield return new WaitForSecondsRealtime(0.15f);
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

	public IEnumerator DisplayQuestion(List<DialogueTrigger.DialogueResponse> responses)
	{
		choiceTextFadeable.SelfFadeIn(dur:0.2f);

		var choiceTransform = choiceBar.transform as RectTransform;
		choiceTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,  700.0f);

		float duration = 10.0f;
		float timeElapsed = 0;
		while (timeElapsed < duration)
        {
			if (CrossPlatformInputManager.GetButtonDown("Fire1"))
			{
				yield return DisplayResponse(responses[0]);
				break;
			}
			if (CrossPlatformInputManager.GetButtonDown("Fire2"))
			{
				yield return DisplayResponse(responses[1]);
				break;
			}
			choiceTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,  Mathf.Lerp(700.0f, 20, timeElapsed / duration));
            yield return null;
            timeElapsed += Time.deltaTime;
        }

		if (timeElapsed >= duration)
		{
			yield return DisplayResponse(responses[2]);
		}

		yield return null;
	}

	public IEnumerator DisplayResponse(DialogueTrigger.DialogueResponse response)
	{
		choiceTextFadeable.SelfFadeOut(dur:0.2f);
		yield return dialogueTextFadeable.FadeOut(dur:0.2f);
		LumpyController.instance.StartCoroutine(LumpyController.instance.SetExpression(response.expression));
		yield return DisplayDialogue(response.text);
		yield return new WaitForSeconds(response.delay);
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
