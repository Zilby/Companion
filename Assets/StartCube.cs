using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartCube : MonoBehaviour
{
	public static StartCube instance;

	public FadeableMesh wall;    

    public Material platformMaterial;

    public Material roomMaterial;

    public Material roomFadeMaterial;

	public AudioClip clip;

    private void Reset()
	{
        platformMaterial = Resources.Load<Material>("Platform");
		roomMaterial = Resources.Load<Material>("Room");
		roomFadeMaterial = Resources.Load<Material>("RoomFade");
        clip = Resources.Load<AudioClip>("Fancy Note");
		
#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}

	private void Awake()
	{
		instance = this;

	}

	private void Start()
	{
		wall.Show();
		wall.GetComponent<MeshRenderer>().sharedMaterial = roomMaterial;
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
		aSource.clip = clip;
		aSource.outputAudioMixerGroup = Resources.Load<AudioMixer>("Mixer").FindMatchingGroups("SFX")[0];
		aSource.Play();
		wall.GetComponent<MeshRenderer>().sharedMaterial = roomFadeMaterial;
		yield return wall.FadeOut(dur: 0.8f);
		yield return GameController.instance.eventBasedDialogues[1].Dialogue();
		yield return new WaitForSeconds(5f);
		Destroy(g);
	}

    public void Disappear()
	{
        Material platformMaterialInstance = new Material(platformMaterial);
        Material roomFadeMaterialInstance = new Material(roomFadeMaterial);
		var fadeableMeshes = GetComponentsInChildren<FadeableMesh>();

        foreach (var fadeableMesh in fadeableMeshes)
        {
            if (fadeableMesh.Renderers[0].sharedMaterial == roomMaterial)
            {
                foreach (Renderer rend in fadeableMesh.Renderers)
                {
                    rend.sharedMaterial = roomFadeMaterialInstance;
                }
            }
            else if (fadeableMesh.Renderers[0].sharedMaterial == platformMaterial)
            {
                foreach (Renderer rend in fadeableMesh.Renderers)
                {
                    rend.sharedMaterial = platformMaterialInstance;
                }
            }
            StartCoroutine(fadeableMesh.FadeOut(dur: 0.8f));
        }
	}
}
