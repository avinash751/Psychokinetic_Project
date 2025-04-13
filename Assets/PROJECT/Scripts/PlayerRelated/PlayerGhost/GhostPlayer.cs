using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostPlayer : MonoBehaviour, IResettable
{
    [SerializeField] Animator animator;
    [Resettable] GameObject g => gameObject;
    Coroutine playback;
    GhostData ghostData;

    private void Start()
    {
        ReferenceManager.Instance.Resetter.OnReset += Initialize;
        transform.position = ReferenceManager.Instance.PlayerTransform.position;
        Initialize();
    }

    void Initialize()
    {
        if (ghostData != null && ghostData.ghostFrames.Count > 0)
        {
            gameObject.SetActive(true);
            if (playback != null) { StopCoroutine(playback); }
            playback = StartCoroutine(Playback());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator Playback()
    {
        Transform player = ReferenceManager.Instance.PlayerTransform;
        Vector3 startPosition = player.position;
        Quaternion startRotation = player.rotation;
        int currentFrame = 0;
        float time = 0;

        while (currentFrame < ghostData.ghostFrames.Count)
        {
            GhostFrame ghostFrame = ghostData.ghostFrames[currentFrame];
            transform.position = Vector3.Lerp(startPosition, ghostFrame.position, time / ghostData.timestep);
            transform.rotation = Quaternion.Slerp(startRotation, ghostFrame.rotation, time / ghostData.timestep);

            if (ghostFrame.animationValues != null)
            {
                foreach (var animationValue in ghostFrame.animationValues)
                {
                    animator.SetFloat(animationValue.key, animationValue.value);
                }
            }

            if (time >= ghostData.timestep)
            {
                startPosition = ghostFrame.position;
                startRotation = ghostFrame.rotation;
                currentFrame++;
                time -= ghostData.timestep;
            }

            time += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void SetGhostData(GhostData ghostData) => this.ghostData = new GhostData(ghostData.timestep, new List<GhostFrame>(ghostData.ghostFrames));
}