using UnityEngine;
using System.Collections;


/// <summary>
/// A simple class that can be inherited to enable FadeIn / FadeOut functionality for a mesh.
/// </summary>
public class FadeableMesh : Fadeable
{
    /// <summary>
    /// The canvas group that will be Faded In/Out
    /// </summary>
    [SerializeField]
	protected MeshRenderer rend;

	[SerializeField]
	protected Collider col;

	public MeshRenderer Rend
    {
        get
        {
            return rend;
        }
    }


    public override float Alpha
    {
        get
        {
			return rend.sharedMaterial.color.a;
        }

        set
        {
			rend.sharedMaterial.color = rend.sharedMaterial.color.A(value);
        }
    }

    public override bool Active
    {
        set
        {
            gameObject.SetActive(value);
        }
    }

    public override bool BlocksRaycasts
    {
        set
        {
			if (col)
			{
				col.enabled = value;
			}
        }
    }

    protected virtual void Reset()
    {
		rend = GetComponent<MeshRenderer>();
        if (rend == null)
        {
			rend = gameObject.AddComponent<MeshRenderer>();
        }
		col = GetComponent<Collider>();

    }
}