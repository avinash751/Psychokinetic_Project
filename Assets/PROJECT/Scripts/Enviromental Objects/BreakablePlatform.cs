using UnityEngine;

public class BreakablePlatform : EnviromentalAid
{
    [SerializeField] float maxTime;
    [Resettable]private Rigidbody PadRB;
    [Resettable]private bool playerOnPad;
    [Resettable]private float timer = 0.0f;

    [SerializeField] Color endColor = Color.white;
    protected override void Start()
    {
        base.Start(); 
        PadRB = GetComponent<Rigidbody>();
        PadRB.useGravity = false;
        PadRB.isKinematic = true;
        padRenderer.material.color = endColor;
    }

    private void Update()
    {
        if (playerOnPad)
        {
            timer += Time.deltaTime;
            if (timer >= maxTime)
            {
                PadRB.useGravity = true;
                PadRB.isKinematic = false;
            }
            else
            {
                float ratio = 1f - (timer / maxTime);

                Color activationColor = Color.Lerp(endColor, activatedCol, ratio);

                if (padRenderer.material.HasProperty("_BaseColor"))
                { padRenderer.material.SetColor("_BaseColor",activationColor); }
                else if (padRenderer.material.HasProperty("_Color"))
                { padRenderer.material.color = activationColor; }

            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerRB.gameObject)
        {
            playerOnPad = true;
            ActivationAndVfx(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerRB.gameObject)
        {
            playerOnPad = false;
            ActivationAndVfx(false);
        }
    }

    
}

