using UnityEngine;
using Cinemachine;

public class InstantCameraMove : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook[] virtualCameras;
    Vector3 startPos;
    Quaternion startRot;

    void Start()
    {
        if (virtualCameras == null ||  virtualCameras.Length == 0)
        {
            Debug.LogError("Please attach both freelook virtual cameras.");
            return;
        }

        startPos = transform.position;
        startRot = transform.rotation;

        ReferenceManager.Instance.Resetter.OnReset += () => MoveCamera();
    }

    void MoveCamera()
    {
        foreach (var cam in virtualCameras)
        {
            cam.ForceCameraPosition(startPos, startRot);
        }
    }
}