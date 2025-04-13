using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BippiFeedbacks : MonoBehaviour
{
    [SerializeField] BippiTutorialNavigator bippiNavi;
    [SerializeField] private Animator bippiAnim;

    string BippiCallAudioKey = "BippiCall";
    private void OnEnable()
    {
        if (bippiNavi != null)
        {
            bippiNavi.BippiIsOnTheMove += BippiMoving;
            bippiNavi.OnBippiDestinationArrived += BippiWaving;
            bippiNavi.OnBippiIdle += BippiIdle;
        }
    }


    private void OnDisable()
    {
        if (bippiNavi != null)
        {
            bippiNavi.BippiIsOnTheMove -= BippiMoving;
            bippiNavi.OnBippiDestinationArrived -= BippiWaving;
            bippiNavi.OnBippiIdle -= BippiIdle;
        }
    }

    private void BippiMoving()
    {
        bippiAnim.SetInteger("State", 3);
        bippiAnim.SetBool("Moving", true);
    }

    private void BippiWaving()
    {
        bippiAnim.SetInteger("State", 2);
        bippiAnim.SetBool("Moving", false);
        AudioManager.Instance.PlayAudio(BippiCallAudioKey);
    }

    private void BippiIdle()
    {
        bippiAnim.SetInteger("State", 1);
        bippiAnim.SetBool("Moving", true);
    }

}
