using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] float shakeTime = 0.2f;

    private float timer;
    private CinemachineBasicMultiChannelPerlin _chmcp;

    void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    public void Start()
    {
        StopShake();
    }
    public void ShakeCamera(float shakeIntensity)
    {
        CinemachineBasicMultiChannelPerlin _chmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _chmcp.m_AmplitudeGain = shakeIntensity;
        timer = shakeTime;
        Debug.Log("shake");
    }
    public void StopShake()
    {
        CinemachineBasicMultiChannelPerlin _chmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _chmcp.m_AmplitudeGain = 0f;
        timer = 0;
    }

    void Update()
    {
        if(Input .GetKey(KeyCode.F)) { ShakeCamera(2); }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) StopShake();
        }
    }
}
