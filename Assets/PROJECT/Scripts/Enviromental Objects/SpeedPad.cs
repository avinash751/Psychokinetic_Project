using CustomInspector;
using System.Collections;
using UnityEngine;

public class SpeedPad : EnviromentalAid
{
    [SerializeField] private float groundspeedBoost = 10f;
    [SerializeField] private float railSpeedBoost;
    [ReadOnly][SerializeField] private float OgRailSpeed;
    [ReadOnly][SerializeField][Resettable] private float currentRailSpeed;

    [SerializeField] private Animator ringanim;


    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PlayerMovement player)) return;
        if (playerRB == null || isActivated) return;
        ActivationAndVfx(true);

        if (grindController != null && PlayerMovement.moveState is MoveState.OnRail)
        {
            if (ringanim != null)
            { ringanim.SetBool("Spin", true); }
            OgRailSpeed = grindController.currentGrindSpeed;
            grindController.currentGrindSpeed += railSpeedBoost;
            currentRailSpeed = grindController.currentGrindSpeed;
            return;
        }

        Vector3 boostForce = transform.forward * groundspeedBoost;
        playerRB.AddForce(boostForce, ForceMode.VelocityChange);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerMovement player)) return;
        ActivationAndVfx(false);
        if (grindController == null || PlayerMovement.moveState is not MoveState.OnRail) return;
        ringanim.SetBool("Spin", false);

        if (OgRailSpeed == 0)
        { grindController.currentGrindSpeed = grindController.normalGrindSpeed; }
        else
        {
            grindController.currentGrindSpeed = OgRailSpeed;
            currentRailSpeed = 0;
        }    
    }
}
