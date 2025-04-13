using UnityEngine;
using System.Collections;
using System;

public class Stomp : PlayerMovement
{
    [SerializeField] float finalStompForce = 10f;
    [SerializeField] float stompDuration;
    [SerializeField] AnimationCurve stompCurve;

    string key = "Stomp";
    public static Action OnStomp;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (verticalState is VerticalState.Grounded) return;
        if (InputManager.GetKeyState("Stomp"))
        {
            StartCoroutine(StompDown());
            OnStomp?.Invoke();
            AudioManager.Instance?.PlayAudio(key);
        }
    }

    IEnumerator StompDown()
    {
        float elapsedTime = 0;
        float currentStompForce = 0;
        while (elapsedTime < stompDuration)
        {
            if (verticalState is VerticalState.Grounded) break;

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / stompDuration;
            currentStompForce = Mathf.Lerp(0, finalStompForce,stompCurve.Evaluate(t));
            Rb.AddForce(Vector3.down * currentStompForce, ForceMode.Impulse);
            SetJumpStateTo(JumpState.Stomping);
            yield return null;
        }     
    }
}
