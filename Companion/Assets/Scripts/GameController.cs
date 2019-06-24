using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
	public static GameController instance;

	public GameObject lumpy;

	public FadeableMesh wall;

	public List<DialogueTrigger> eventBasedDialogues;
	
	[Range(0, 2)]
	public float timescale = 1;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		wall.Show();
		wall.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Room");
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
		GameObject g = new GameObject();
		AudioSource aSource = g.AddComponent<AudioSource>();
		g.transform.position = wall.transform.position;
		aSource.spatialBlend = 1;
		aSource.volume = 1;
		aSource.rolloffMode = AudioRolloffMode.Linear;
		aSource.clip = Resources.Load<AudioClip>("Fancy Note");
		aSource.outputAudioMixerGroup = Resources.Load<AudioMixer>("Mixer").FindMatchingGroups("SFX")[0];
		aSource.Play();
		wall.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("RoomFade");
		yield return wall.FadeOut(dur: 0.8f);
		yield return eventBasedDialogues[1].Dialogue();
		yield return new WaitForSeconds(5f);
		Destroy(g);
	}
}
