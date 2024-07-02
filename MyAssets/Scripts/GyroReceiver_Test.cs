using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using OscSimpl;

public class GyroReceiver_Test : MonoBehaviour
{
    public Transform phone;  // 3D 모델의 Transform
    public float motionDelay = 0.1f;  // 회전 시 딜레이
    public TextMeshProUGUI gyroText;  // UI 텍스트 (선택사항)
    
    private float x, y, z, w;
    private Quaternion initialRotation;

    [Header("OSC Settings")]
    public string address = "/gyro";  // OSC 주소
    public int port = 17701;  // 포트 번호

    private OscIn oscIn;
    private UdpReceiver udpReceiver;

    void Start()
    {
        oscIn = gameObject.AddComponent<OscIn>();
        oscIn._port = port;
        //oscIn.Bind(address, OnMessageReceived);

        initialRotation = Quaternion.identity;
    }

    public void OnClickCalibrationButton()
    {
        initialRotation = new Quaternion(x, y, z, w);
    }

    // private void OnMessageReceived(OSCMessage message)
    // {
    //     if (message.ToFloat(out float[] values) && values.Length == 4)
    //     {
    //         x = values[0];
    //         y = values[1];
    //         z = values[2];
    //         w = values[3];

    //         UpdateGyro();
    //     }
    // }

    private void UpdateGyro()
    {
        gyroText.text = "x: " + x + "\ny: " + y + "\nz: " + z + "\nw: " + w;  
        Quaternion resultRotation = Quaternion.Inverse(initialRotation) * new Quaternion(x, y, z, w);
        phone.DORotateQuaternion(resultRotation, motionDelay).OnUpdate(UpdateRotateText);
    }

    private void UpdateRotateText()
    {
        // 회전 업데이트 텍스트 갱신 (선택사항)
    }
}
