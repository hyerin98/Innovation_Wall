using UnityEngine;
using TMPro;
using DG.Tweening;
using OscSimpl;
using IMFINE.Utils.ConfigManager;
using System.Collections;
using UnityEngine.UI;
using Unity.Mathematics;

public class GyroReceiver_Test : MonoBehaviour
{
    [Header("Phone Settings")]
    public GameObject phonePrefab;
    public Transform phone; 
    public GameObject spawnPhone;
    public float motionDelay = 0.5f;  

    [Header("Animation Settings")]
    Tweener tweener;
    public Animation anim;
    public int animationStartDistance;
    public int animationEndDistance;

    [Header("UI Settings")]
    public TextMeshProUGUI RotText;  
    public TextMeshProUGUI gyroText;
    public Button resetButton; 
    
    [Header("OSC Settings")]
    private float x, y, z, w;
    private Quaternion initialRotation;
    public string address = "/gyro";  
    public int port = 17701;  
    private OscIn oscIn;

    void Start()
    {
        oscIn = gameObject.AddComponent<OscIn>(); 
        oscIn.Open(port); 
         
        //initialRotation = phone.rotation; // 7.4 pc와 모바일의 방향이 달랐던 원인
        initialRotation = new Quaternion(x, y, z, w); // 7.4 위의 코드를 이렇게 초기화해주면 pc와 모바일의 방향이 같게 움직인다
        oscIn.Map(address, OnMessageReceived);
        resetButton.onClick.AddListener(ResetRotation);        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && phone.gameObject.CompareTag("Phone"))
        {
            Vector3 initialPosition = phone.position;
            phone.DOMoveX(-2.5f, 2f); 
            phone.gameObject.tag = "notPhone";
            
            GameObject newPhone = Instantiate(spawnPhone, new Vector3(2f, 4.75f, 0f), Quaternion.identity);
            newPhone.tag = "Phone";
            newPhone.transform.DOMove(initialPosition, 2f);
        }
    }

    public void ResetRotation()
    {
        initialRotation = new Quaternion(x, y, z, w);
    }

    void OnMessageReceived(OscMessage message)
    {
        Debug.Log("OSC Message Received");

        if (message.Count() != 4) return;

        if (ConfigManager.instance.data.showGyroLog)
            TraceBox.Log("[R]: " + message);

        if (phone.gameObject.CompareTag("Phone"))
        {
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
        }
        OscPool.Recycle(message);
    }
    

    private void UpdateGyro()
    {
        motionDelay = ConfigManager.instance.data.motionDelay;
        gyroText.text = $"x: {x}\ny: {y}\nz: {z}\nw: {w}"; 

        Quaternion resultRotation = Quaternion.Inverse(initialRotation) * new Quaternion(x, y, z, w);
        //phone.DORotateQuaternion(resultRotation, motionDelay).OnUpdate(UpdateRotateText);
        tweener.SetEase(AnimationCurves.instance.curves[0]);
        tweener = phone.DORotateQuaternion(resultRotation, motionDelay).OnUpdate(UpdateRotateText);
    }

    private void UpdateRotateText()
    {
        Vector3 eulerAngles = phone.rotation.eulerAngles;
        float xAngle = Mathf.Round(eulerAngles.x * 100f) / 100f;
        float yAngle = Mathf.Round(eulerAngles.y * 100f) / 100f;
        float zAngle = Mathf.Round(eulerAngles.z * 100f) / 100f;

        string formattedAngles = $"({xAngle:F2} , {yAngle:F2} , {zAngle:F2})";

        RotText.text = $"Rot: {formattedAngles}";
    }
}
