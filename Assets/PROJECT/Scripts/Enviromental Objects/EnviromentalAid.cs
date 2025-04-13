
using CustomInspector;
using UnityEngine;


public abstract class EnviromentalAid : MonoBehaviour, INeedPlayerRefs, IResettable
{
    protected Rigidbody playerRB;
    protected PlayerMovement player;
    protected GrindController grindController;

    [Resettable][SerializeField] protected MeshRenderer padRenderer;
    [Resettable] protected Transform ObjectTransform => transform;
    [ReadOnly][Resettable][SerializeField] protected bool isActivated;

    protected Color unactivatedCol = Color.white;
    [SerializeField]protected Color activatedCol = Color.red;

    protected virtual void Start()
    {
        if(padRenderer == null)
        {
            TryGetComponent(out padRenderer);
        }
        
        playerRB = ReferenceManager.Instance.PlayerRb;
        player = ReferenceManager.Instance.PlayerMovement;
        grindController = ReferenceManager.Instance.PlayerGC;

        // Check if the material has the toon shader property "_BaseColor", if not, set the color directly
       GetUnActivatedColor();
    }

    private void GetUnActivatedColor()
    {
        if(padRenderer == null) { return; }
        if (padRenderer.material.HasProperty("_BaseColor"))
        { unactivatedCol = padRenderer.material.GetColor("_BaseColor"); }
        else if (padRenderer.material.HasProperty("_Color"))
        { unactivatedCol = padRenderer.material.color; }
    }

    protected void ActivationAndVfx(bool _isActivated)
    {
        isActivated = _isActivated;
        Color activationColor = _isActivated ? activatedCol : unactivatedCol;

        // Check if the material has the toon shader property "_BaseColor", if not, set the color directly
        if (padRenderer == null) { return; }
        if (padRenderer.material.HasProperty("_BaseColor"))
        { padRenderer.material.SetColor("_BaseColor", activationColor); }
        else if (padRenderer.material.HasProperty("_Color"))
        { padRenderer.material.color = activationColor; }
    }
}
