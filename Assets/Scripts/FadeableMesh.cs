using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A simple class that can be inherited to enable FadeIn / FadeOut functionality for a mesh.
/// </summary>
public class FadeableMesh : Fadeable
{
    [SerializeField]
	protected List<MeshRenderer> renderers;

    [SerializeField]
	protected List<ParticleSystemRenderer> particleRenderers;

	[SerializeField]
	protected List<Collider> cols;

    protected float alpha;

	public List<MeshRenderer> Renderers
    {
        get
        {
            return renderers;
        }
    }

    public List<ParticleSystemRenderer> ParticleRenderers
    {
        get
        {
            return particleRenderers;
        }
    }


    public override float Alpha
    {
        get
        {
            return alpha;
        }

        set
        {
            alpha = value;
            foreach (MeshRenderer rend in renderers)
            {
                if (rend.sharedMaterial != null)
                {
			        rend.sharedMaterial.color = rend.sharedMaterial.color.A(alpha);
                }
            }
            foreach (ParticleSystemRenderer rend in particleRenderers)
            {
                if (rend.sharedMaterial != null)
                {
			        rend.sharedMaterial.color = rend.sharedMaterial.color.A(alpha);
                }
            }
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
			foreach (Collider col in cols)
            {
                if (col != null)
                {
				    col.enabled = value;
                }
			}
        }
    }

    protected virtual void Reset()
    {
        renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        particleRenderers = new List<ParticleSystemRenderer>(GetComponentsInChildren<ParticleSystemRenderer>());
		cols = new List<Collider>(GetComponentsInChildren<Collider>()); 
    }

    protected override void Awake()
    {
        alpha = 0;
        foreach (MeshRenderer rend in renderers)
        {
            if (rend.sharedMaterial != null)
            {
                alpha = rend.sharedMaterial.color.a;
                break;
            }
        }
        base.Awake();
    }
}