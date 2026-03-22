using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;
    private CinemachineBasicMultiChannelPerlin camShaker;

    private float timerCurr;
    private float timerStart;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        camShaker = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public static void ShakeCamera(float amplitude, float duration, bool isImpactFrame)
    {
        instance.camShaker.m_AmplitudeGain = amplitude;
        instance.timerStart = duration;
        instance.timerCurr = instance.timerStart;
        instance.StartCoroutine(instance.OnImpactFrame(duration));
    }

    // Update is called once per frame
    void Update()
    {
        if (timerCurr > 0)
        {
            timerCurr -= Time.deltaTime;
            Mathf.Lerp(timerStart, 0f, timerStart / timerCurr);

            if (timerCurr <= 0)
            {
                camShaker.m_AmplitudeGain = 0f;
            }
        }
    }

    public IEnumerator OnImpactFrame(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
