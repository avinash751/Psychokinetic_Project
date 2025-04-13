using CustomInspector;
using UnityEngine;

public class RailSpeedMultiplier : EnviromentalAid, IResettable
{
    [SerializeField] float railSpeedMultiplier = 1.5f;
    [SerializeField][Resettable] float currentRailSpeed;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(OnRailsExit), 0.3f, 0.3f);
    }

    private void OnCollisionEnter(Collision collision)
    {      
        if (collision.transform.TryGetComponent(out PlayerMovement player) && !isActivated)
        {         
            grindController.currentGrindSpeed *= railSpeedMultiplier;
            currentRailSpeed = grindController.currentGrindSpeed;
            ActivationAndVfx(true);
        }
    }

    void OnRailsExit()
    {
        if(PlayerMovement.grindState == GrindState.None || PlayerMovement.grindState == GrindState.RailSwitching && isActivated)
        {
            grindController.currentGrindSpeed = grindController.normalGrindSpeed;
            currentRailSpeed = 0;     
            ActivationAndVfx(false);
        }
    }
}
