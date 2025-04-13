using Dreamteck.Splines;
using UnityEngine;
using Cinemachine;

public class ReferenceManager : MonoBehaviour, IResettable
{
    public static ReferenceManager Instance { get; private set; }

    // All the references that are needed in the game, will be set in the Awake method

    [Resettable][field: SerializeField] public Rigidbody PlayerRb { get; private set; }
    [Resettable][field: SerializeField] public SplineFollower PlayerSF { get; private set; }
    [Resettable][field: SerializeField] public Transform PlayerTransform { get; private set; }
    [field: SerializeField] public Transform MainCamTransform { get; private set; }
    [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
    [field: SerializeField] public GrindController PlayerGC { get; private set; }
    [field: SerializeField] public Telekinesis telekinesis { get; private set; }
    [field: SerializeField] public Resetter Resetter { get; private set; }
    [field:SerializeField] public LevelManager LevelManager { get; private set; }
    [field:SerializeField] public GameTimer GameTimer { get; private set; }
    [field: SerializeField] public CursorSettings CursorSettings { get; private set; }
    [field: SerializeField] public UiManager UiManager { get; private set; }
    [field:SerializeField] public PlayerHealth PlayerHealth { get; private set; }
    [field:SerializeField] public SaveManager SaveManager { get; private set; }
    [field:SerializeField] public GameManager GameManager { get; private set; }
    [field: SerializeField] public Jump JumpScript { get; private set; }

    [field: SerializeField] public CinemachineFreeLook keyboardAnsMouseCamera { get; private set; }
    [field: SerializeField] public CinemachineFreeLook controllerCamera { get; private set; }

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Awake()
    {
        PlayerGC = FindObjectOfType<GrindController>();
        PlayerMovement = FindObjectOfType<PlayerMovement>();
        PlayerRb = PlayerMovement.GetComponent<Rigidbody>();
        PlayerSF = PlayerMovement.GetComponent<SplineFollower>();
        PlayerTransform = PlayerMovement.transform;
        MainCamTransform = Camera.main.transform;
        telekinesis = PlayerTransform.GetComponent<Telekinesis>();
        Resetter = FindObjectOfType<Resetter>();
        LevelManager = FindObjectOfType<LevelManager>();
        GameManager = FindObjectOfType<GameManager>();
        CursorSettings = FindObjectOfType<CursorSettings>();
        UiManager = FindObjectOfType<UiManager>();
        PlayerHealth = FindObjectOfType<PlayerHealth>();
        SaveManager = FindObjectOfType<SaveManager>();
        GameTimer = FindObjectOfType<GameTimer>();
        keyboardAnsMouseCamera = FindFirstObjectByType<CameraAimAndFollowAttachmentChange>().transform.GetChild(1).GetComponent<CinemachineFreeLook>();
        controllerCamera = FindFirstObjectByType<CameraAimAndFollowAttachmentChange>().transform.GetChild(0).GetComponent<CinemachineFreeLook>();
        JumpScript = FindObjectOfType<Jump>();
    }
}
