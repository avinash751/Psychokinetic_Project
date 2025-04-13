using System.Collections;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour, IResettable
{
    private Animator animator;
    private Jump jumpScript;
    [SerializeField] private float accelerate = 3f;
    [SerializeField] private float slowdown = 3f;
    [SerializeField] private float threshold = 0.01f;


    [Resettable] private float jumpingState;
    [Resettable] private float grindState;
    [Resettable] private float specialJumpState;
    [Resettable] private float telekState;
    [Resettable] private bool throwing;
    [Resettable] private bool specialJumping;
    [SerializeField] [Resettable] private float targetJumpState;

    private float velo = 0.0f;
    private Rigidbody prb;
    private int veloHash;
    

    private Telekinesis telekenesis;
    [SerializeField] private float animationLength;

 
    void Start()
    {
        jumpScript = ReferenceManager.Instance.JumpScript;
        animator = GetComponent<Animator>();
        veloHash = Animator.StringToHash("Velocity"); 
        prb = ReferenceManager.Instance.PlayerRb;
        telekenesis = ReferenceManager.Instance.telekinesis;
        EventManager.Subscribe<TelekineticObject>(EventType.Telekinesis, _ => AnimateThrow());
    }

    void Update()
    {
        //Debug.Log(specialJumpState);
        IdleWalkRun();
        HandleVerticalAndJumpState();
        HandleGrindState();
        HandleTeleState();
        DetectLeanInput();
        SpecialJumpStates();
    }

    private void IdleWalkRun()
    {
        if(PlayerMovement.moveState == MoveState.OnRail)
        {
            animator.SetFloat("Velocity", 1);
            return;
        }

        if (prb.linearVelocity.magnitude > threshold && velo < 1.0f)
        {
            velo += Time.deltaTime * accelerate;
        }
        else if (prb.linearVelocity.magnitude <= threshold && velo > 0.0f)
        {
            velo -= Time.deltaTime * slowdown;
        }

        animator.SetFloat(veloHash, velo);
    }
    private void HandleVerticalAndJumpState()
    {
        switch (PlayerMovement.verticalState)
        {
            case VerticalState.Grounded:
                jumpingState = 0f;        
                if (jumpScript.bunnyHopsDone == 0)
                {
                    targetJumpState = 0f;
                }
                break;

            case VerticalState.InAir:
                jumpingState = 1f; 
                break;

            case VerticalState.Falling:
                if (specialJumpState > 0)
                {
                    return;
                }
                else
                {
                    jumpingState = 2f; // Falling
                }
                break;
        }
        animator.SetFloat("JumpState", jumpingState);
        animator.SetFloat("SpecialJumpState", targetJumpState);
    }
    
    private void SpecialJumpStates()
    {
        if (jumpingState == 1f)
        {
            switch (PlayerMovement.jumpState)
            {
                case JumpState.NormalJump:
                    jumpingState = 1f;
                    break;

                case JumpState.BunnyHopping:
                    jumpingState = 1.5f;
                    SpecialJumpChecker();
                    break;

                case JumpState.Stomping:

                case JumpState.PerfectStomp:
                    jumpingState = 2.5f;
                    break;
            }
        }
        animator.SetFloat("JumpState", jumpingState);
    }

    private void SpecialJumpChecker()
    {
        if (jumpScript.bunnyHopsDone == 0)
        {
            jumpingState = 1f;
            specialJumpState = 0f;
            targetJumpState = 0f;
        }

        if (jumpScript.bunnyHopsDone == 1)
        {
            jumpingState = 1.5f;
            specialJumpState = 0.2f;
        }
        if (jumpScript.bunnyHopsDone == 2 || jumpScript.bunnyHopsDone == 3 || jumpScript.bunnyHopsDone == 4)
        {
            jumpingState = 1.5f;
            specialJumpState = 0.5f;
        }
        float currentJumpState = animator.GetFloat("SpecialJumpState");
        targetJumpState = Mathf.Lerp(currentJumpState, specialJumpState, Time.deltaTime * 1.8f);
        animator.SetFloat("JumpState", jumpingState);
        animator.SetFloat("SpecialJumpState", targetJumpState);
    }

    private void HandleGrindState()
    {
        switch (PlayerMovement.grindState)
        {
            case GrindState.GettingOnRail:
                grindState = 0.2f;
                break;

            case GrindState.RailGrinding:
                grindState = 1f;
                break;

            case GrindState.RailBreaking:
                grindState = 1f;
                break;

            case GrindState.None:
                grindState = 0f;
               break;
        }
        animator.SetFloat("Grinding", grindState);
    }

    private void HandleTeleState()
    {
        switch (telekenesis.State)
        {
            case TelekinesisState.Idle:
                if (throwing)
                {
                    telekState = 1f;
                }
                else
                {
                    telekState = 0f;
                }
                break;

            case TelekinesisState.Grabbing:
                telekState = 0.5f;
                break;
        }
        animator.SetFloat("TelekState", telekState);
    }

    private void DetectLeanInput()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float leanAngle = Vector3.Dot(transform.right, inputDirection);

        float normalizedLeanAngle = Mathf.InverseLerp(-1f, 1f, leanAngle);
        float smoothedLean = Mathf.Lerp(animator.GetFloat("LeanState"), normalizedLeanAngle, Time.deltaTime * 1f);

        animator.SetFloat("LeanState", smoothedLean);
    }

   

    private void AnimateThrow()
    {
        throwing = true;
        StopAllCoroutines();
        StartCoroutine(ManageThrowAnim());
    }

    IEnumerator ManageThrowAnim()
    {
        float timer = 0f;
        while (timer < animationLength && telekenesis.State == TelekinesisState.Idle)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        throwing = false;
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<TelekineticObject>(EventType.Telekinesis, _ => AnimateThrow());
    }

    private void ForcePlay()
    {
        specialJumping = true;
    }

    private void StopForcePlay()
    {
        specialJumping = false;
    }
}