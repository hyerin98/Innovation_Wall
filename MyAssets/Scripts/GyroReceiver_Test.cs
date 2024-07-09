using UnityEngine;
using TMPro;
using DG.Tweening;
using OscSimpl;
using IMFINE.Utils.ConfigManager;
using System.Collections.Generic;
using UnityEngine.UI;

public class GyroReceiver_Test : MonoBehaviour
{
    [Header("Phone Settings")]
    public List<GameObject> phones = new List<GameObject>();
    private Vector3 offscreenPosition = new Vector3(-2.5f, 4.75f, 0f);
    private Vector3 startPosition = new Vector3(1.74f, 4.75f, 0f);
    private int currentPhoneIndex = 0;

    [Header("Animation Settings")]
    public Ease ease;
    Tweener tweener;
    public Animator animator;
    public int animationStartDistance;
    public int animationEndDistance;
    public float motionDelay = 0.1f;
    float resetRotationTween = 0.5f;


    [Header("UI Settings")]
    public TextMeshProUGUI RotText;
    public TextMeshProUGUI gyroText;
    public Button resetButton;
    public GameObject DebugCanvas;
    public bool isDebugMode;

    [Header("OSC Settings")]
    private float x, y, z, w;
    private Quaternion initialRotation;
    public string address = "/gyro";
    public int port = 17701;
    private OscIn oscIn;

    private void Awake()
    {
        InitializePhonePositions();
        
    }

    private void Start()
    {
        oscIn = gameObject.AddComponent<OscIn>();
        oscIn.Open(port);

        ResetRotation();
        oscIn.Map(address, OnMessageReceived);
        resetButton.onClick.AddListener(ResetRotation);
    }

    void InitializePhonePositions()
    {
        foreach (var phone in phones)
        {
            phone.transform.position = offscreenPosition;
        }
        if (phones.Count > 0)
        {
            phones[0].transform.position = new Vector3(0f, 4.75f, 0f);
            phones[0].tag = "Phone";
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SwitchPhones();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isDebugMode)
        {
            isDebugMode = false;
            Debug.Log("KeyDown Space");
            DebugCanvas.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isDebugMode)
        {
            isDebugMode = true;
            Debug.Log("Reply KeyDown Space");
            DebugCanvas.SetActive(true);
        }

    }

    void SwitchPhones()
    {
        var currentPhone = phones[currentPhoneIndex];
        currentPhone.transform.DOMove(offscreenPosition, 1f);
        currentPhone.gameObject.tag = "notPhone";
        currentPhone.transform.rotation = Quaternion.identity; // 실제 움직이고 있던 폰 로테이션 초기화

        currentPhoneIndex = (currentPhoneIndex + 1) % phones.Count;
        var nextPhone = phones[currentPhoneIndex];
        nextPhone.transform.position = startPosition;
        nextPhone.tag = "Phone";
        nextPhone.transform.DOMove(new Vector3(0f, 4.75f, 0f), 1f);
        nextPhone.transform.DORotate(new Vector3(0f, 360f, 0f), 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() => Debug.Log("회전 완료!"));
    }

    public void ResetRotation()
    {
        resetRotationTween = ConfigManager.instance.data.resetRotationTween;

        initialRotation = new Quaternion(x, y, z, w);
        transform.DORotateQuaternion(initialRotation,resetRotationTween);
    }

    void OnMessageReceived(OscMessage message)
    {
        //Debug.Log("OSC Message Received");

        if (message.Count() != 4) return;

        if (ConfigManager.instance.data.showGyroLog)
            TraceBox.Log("[R]: " + message);

        if (message.TryGet(0, out x) &&
            message.TryGet(1, out y) &&
            message.TryGet(2, out z) &&
            message.TryGet(3, out w))
        {
            UpdateGyro();
        }
        else
        {
            Debug.LogWarning("Failed to parse OSC message.");
        }

        OscPool.Recycle(message);
    }

    private void UpdateGyro()
    {
        motionDelay = ConfigManager.instance.data.motionDelay;
        gyroText.text = $"x: {x}\ny: {y}\nz: {z}\nw: {w}";

        Quaternion resultRotation = Quaternion.Inverse(initialRotation) * new Quaternion(x, y, z, w);
        //tweener.SetEase(AnimationCurves.instance.curves[0]);
        //tweener = phones[currentPhoneIndex].transform.DORotateQuaternion(resultRotation, motionDelay).SetEase(Ease.Linear).OnUpdate(UpdateRotateText);
        //phones[currentPhoneIndex].transform.DORotateQuaternion(resultRotation,motionDelay).OnUpdate(UpdateRotateText);
        //Vector3 eulerAngles = phones[currentPhoneIndex].transform.rotation.eulerAngles;

        Tween rotateTween = phones[currentPhoneIndex].transform.DORotateQuaternion(resultRotation, motionDelay);
        //rotateTween.SetEase(AnimationCurves.instance.curves[0]);
        rotateTween.OnUpdate(UpdateRotateText);
        Vector3 eulerAngles = phones[currentPhoneIndex].transform.rotation.eulerAngles;

        TriggerAnimationBasedOnAngle(eulerAngles);
    }

    private void TriggerAnimationBasedOnAngle(Vector3 eulerAngles)
    {
        animator = phones[currentPhoneIndex].GetComponent<Animator>();
        float currentSpeed = animator.GetFloat("speed");

        bool shouldPlayReverseAnimation = (eulerAngles.x > 45 && eulerAngles.x < 135) ||
                                          (eulerAngles.x < -45 && eulerAngles.x > -135) ||
                                           (eulerAngles.z > 45 && eulerAngles.z < 135) ||
                                          (eulerAngles.z < -45 && eulerAngles.z > -135) ||
                                         (eulerAngles.y > 45 && eulerAngles.y < 135) ||
                                           (eulerAngles.y < 315 && eulerAngles.y > 225);


        if (shouldPlayReverseAnimation)
        {
            if (currentSpeed >= 0)
            {
                animator.SetFloat("speed", -1f);
                animator.CrossFade("testtest3",0.5f);
                TraceBox.Log("닫어");
            }
        }
        else
        {
            if (currentSpeed < 0)
            {
                animator.SetFloat("speed", 1f);
                animator.CrossFade("testtest1",0.5f);
                TraceBox.Log("열어");
            }
        }
    }


    private void UpdateRotateText()
    {
        Vector3 eulerAngles = phones[currentPhoneIndex].transform.rotation.eulerAngles;
        float xAngle = Mathf.Round(eulerAngles.x * 100f) / 100f;
        float yAngle = Mathf.Round(eulerAngles.y * 100f) / 100f;
        float zAngle = Mathf.Round(eulerAngles.z * 100f) / 100f;

        string formattedAngles = $"({xAngle:F2} , {yAngle:F2} , {zAngle:F2})";

        RotText.text = $"Rot: {formattedAngles}";
    }
}
