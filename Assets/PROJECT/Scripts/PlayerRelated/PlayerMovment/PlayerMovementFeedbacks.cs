using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerMovementFeedbacks : MonoBehaviour
{
    [VInspector.Foldout("Stomping Feedbacks")]
    [SerializeField] ParticleSystem StompParticles;

    [VInspector.Foldout("Rail Grinding Feedbacks")]
    [SerializeField] ParticleSystem GrindTrail;
    [SerializeField] VisualEffect speedLines;

    [VInspector.Foldout("Ground Movement Feedbacks")]
    [SerializeField] ParticleSystem groundDustTrail;
    [SerializeField] ParticleSystem groundDustBurstParticles;
    [SerializeField] Transform groundDustBurstSpawnTransform;

    [VInspector.Foldout("Bunny Hop Feedbacks")]
    [SerializeField] ParticleSystem bunnyHopParticles;
    [SerializeField] Transform bunnyHopParticleSpawnTransform;
    [SerializeField] GameObject wingsParent;
    [SerializeField] GameObject[] wings;
    public Material wingsMat;
    public float startPower = 0.0f;
    public float endPower = 1.0f;
    public float duration = 1.0f;
    private float currentLerpTime = 0.0f;

    private bool isFresnelActive = false;

    private Jump jump;
    float defaultDustTrailRate;
    float defaultGrindTrailRate;
    float defaultSpeedLinesSpawnRate;
    bool spawnedDustBurst;
    bool didJump;

    private void Start()
    {
        wingsParent.SetActive(false);
        foreach (GameObject wing in wings)
        {
            wing.SetActive(false);
        }
        jump = ReferenceManager.Instance.JumpScript;
    }

    private void Update()
    {
        WingLogic();
        ManageGroundMovementFeedbacks();
        ManageGrindVfx();
    }

    private void OnEnable()
    {
        Jump.onSuccessfulBunnyHop += PlayBunnyHopParticles;
        Stomp.OnStomp += ManageStompFeedbacks;
        PlayerMovement.OnJumpStateChanged += PlayPerfectStompParticles;
        PlayerMovement.OnJumpStateChanged += PlayerJumped;

        defaultDustTrailRate = groundDustTrail.emission.rateOverDistance.constant;
        defaultGrindTrailRate = GrindTrail.emission.rateOverDistance.constant;
        defaultSpeedLinesSpawnRate = speedLines.GetFloat("SpawnRate");
    }

    private void OnDisable()
    {
        Jump.onSuccessfulBunnyHop -= PlayBunnyHopParticles;
        PlayerMovement.OnJumpStateChanged -= PlayPerfectStompParticles;
        PlayerMovement.OnJumpStateChanged -= PlayerJumped;
        Stomp.OnStomp -= ManageStompFeedbacks;
    }

    void ManageGroundMovementFeedbacks()
    {
        bool onGround = PlayerMovement.verticalState is VerticalState.Grounded && PlayerMovement.moveState is not MoveState.OnRail;
        bool isBunnyHopping = jump.bunnyHopsDone > 0 || PlayerMovement.jumpState is JumpState.BunnyHopping;

        if (!spawnedDustBurst && !isBunnyHopping && onGround)
        {
            if (groundDustBurstParticles == null || groundDustBurstSpawnTransform == null) { return; }
            ParticleSystem groundDustBurstParticlesInstance = Instantiate(groundDustBurstParticles, groundDustBurstSpawnTransform.position, groundDustBurstParticles.transform.rotation);
            Destroy(groundDustBurstParticlesInstance.gameObject, groundDustBurstParticlesInstance.main.duration + 1);
            spawnedDustBurst = true;
        }

        if (groundDustTrail == null) { return; }
        var emission = groundDustTrail.emission;

        if (onGround)
        {
            emission.rateOverDistance = defaultDustTrailRate;
            return;
        }
        emission.rateOverDistance = 0;
    }

    void ManageGrindVfx()
    {
        if (GrindTrail == null) { return; }

        bool onGrind = PlayerMovement.moveState is MoveState.OnRail;
        bool OnGrounded = PlayerMovement.verticalState is VerticalState.Grounded && PlayerMovement.moveState is not MoveState.OnRail;
        var emission = GrindTrail.emission;

        if (onGrind)
        {
            if (PlayerMovement.grindState is GrindState.RailBreaking)
            {
                emission.rateOverDistance = defaultGrindTrailRate / 2;
            }
            else
            {
                emission.rateOverDistance = defaultGrindTrailRate;
            }

            speedLines.SetFloat("SpawnRate", defaultSpeedLinesSpawnRate);

        }
        else if (OnGrounded)
        {
            emission.rateOverDistance = 0;
            speedLines.SetFloat("SpawnRate", 0);
        }
    }

    void ManageStompFeedbacks()
    {
        StompParticles?.Play();
    }

    void PlayBunnyHopParticles()
    {
        if (bunnyHopParticles == null) { return; }
        ParticleSystem bunnyHopParticlesInstance = Instantiate(bunnyHopParticles, bunnyHopParticleSpawnTransform.position, bunnyHopParticles.transform.rotation);
        Destroy(bunnyHopParticlesInstance.gameObject, bunnyHopParticlesInstance.main.duration + 1);
    }

    void PlayPerfectStompParticles(JumpState state)
    {
        if (state == JumpState.PerfectStomp)
        {
            if (StompParticles != null)
            {
                StompParticles.Play();
            }
        }
    }

    void WingLogic()
    {
        if (wingsParent == null || wings == null || wings.Length == 0) { return; }

        if (jump.bunnyHopsDone == 0 || PlayerMovement.jumpState != JumpState.BunnyHopping)
        {
            if (wingsParent.activeSelf)
            {
                StartCoroutine(FadeOutWingsMaterial());
            }
        }
        else
        {
            wingsParent.SetActive(true);
            for (int i = 0; i < wings.Length; i++)
            {
                wings[i].SetActive(i < Mathf.FloorToInt(jump.bunnyHopsDone));
            }
        }
    }

    IEnumerator FadeOutWingsMaterial()
    {
        if (!isFresnelActive)
        {
            isFresnelActive = true;
            currentLerpTime = 0.0f;

            while (currentLerpTime < duration)
            {
                currentLerpTime += Time.deltaTime;

                float t = currentLerpTime / duration;
                float fresnelPower = Mathf.Lerp(startPower, endPower, t);

                wingsMat.SetFloat("_Fresnel_Power", fresnelPower);

                yield return null; 
            }
            wingsMat.SetFloat("_Fresnel_Power", endPower);

 
            wingsParent.SetActive(false);
            foreach (GameObject wing in wings)
            {
                wing.SetActive(false);
            }

            wingsMat.SetFloat("_Fresnel_Power", startPower);
            isFresnelActive = false;
        }
    }

    void PlayerJumped(JumpState state)
    {
        if (state == JumpState.NormalJump)
        {
            didJump = true;
            spawnedDustBurst = false;
        }
    }
}
