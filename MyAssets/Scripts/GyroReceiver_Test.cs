using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using OscSimpl;
using IMFINE.Utils.ConfigManager;

public class GyroReceiver_Test : MonoBehaviour
{
    public Transform phone; 
    
    public float motionDelay = 0.1f;  
    public TextMeshProUGUI gyroText;  

    private float x, y, z, w;
    private Quaternion initialRotation;

    [Header("OSC Settings")]
    public string address = "/gyro";  
    public int port = 17701;  

    private OscIn oscIn;

    void Start()
    {
        oscIn = gameObject.AddComponent<OscIn>(); 
        oscIn.Open(port); 
        oscIn.Map(address, OnMessageReceived); 

        initialRotation = Quaternion.identity; 
    }

    void Upate()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            phone.DOMoveX(-2.5f, 2f);
            Destroy(phone, 2f);
        }
    }


    void OnMessageReceived(OscMessage message)
    {
        Debug.Log("OSC Message Received");

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
        gyroText.text = "x: " + x + "\ny: " + y + "\nz: " + z + "\nw: " + w; 

        Quaternion resultRotation = Quaternion.Inverse(initialRotation) * new Quaternion(x, y, z, w);
        phone.DORotateQuaternion(resultRotation, motionDelay).OnUpdate(UpdateRotateText);
    }

    private void UpdateRotateText()
    {
        // 회전 업데이트 텍스트 갱신
    }
}
