using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour, IResettable
{
    [SerializeField] Animator animator;
    [SerializeField] float timestep = 0.1f;
    List<GhostFrame> ghostFrames = new();

    private void Start()
    {
        if (animator == null)
        {
            animator = ReferenceManager.Instance.PlayerTransform.GetComponentInChildren<Animator>();
        }
        StartCoroutine(RecordPlayer());
        ReferenceManager.Instance.Resetter.OnReset += () => StartCoroutine(RecordPlayer());
    }

    public IEnumerator RecordPlayer()
    {
        if (ghostFrames.Count > 0) { ghostFrames.Clear(); }
        while (true)
        {
            List<KeyValuePair> animationValues = new()
            {
                new KeyValuePair("Velocity", animator.GetFloat("Velocity")),
                new KeyValuePair("JumpState", animator.GetFloat("JumpState")),
                new KeyValuePair("Grinding", animator.GetFloat("Grinding")),
                new KeyValuePair("TelekState", animator.GetFloat("TelekState")),
                new KeyValuePair("LeanState", animator.GetFloat("LeanState"))
            };
            ghostFrames.Add(new(animator.transform.position, animator.transform.rotation, animationValues));
            yield return new WaitForSeconds(timestep);
        }
    }

    public GhostData GetRecording()
    {
        StopAllCoroutines();
        return new(timestep, ghostFrames);
    }
}