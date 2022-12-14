using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioSource runningSound;
    public float runningMaxVolume;
    public float runningMaxPitch;
    public AudioSource reverseSound;
    public float reverseMaxVolume;
    public float reverseMaxPitch;
    public AudioSource idleSound;
    public float idleMaxVolume;
    public float speedRatio;
    private float revLimiter;
    public float LimiterSound = 1f;
    public float LimiterFrequency = 3f;
    public float LimiterEngage = 0.8f;
    public bool isEngineRunning = false;

    public AudioSource startingSound;
    public AudioSource SlipSource;

    private CarController carController;
    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
        idleSound.volume = 0;
        runningSound.volume = 0;
        reverseSound.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float speedSign=0;
        StartSkid();
        if (carController)
        {
            speedSign = Mathf.Sign(carController.GetSpeedRatio());
            speedRatio = Mathf.Abs(carController.GetSpeedRatio());
        }
        if (speedRatio > LimiterEngage)
        {
            revLimiter = (Mathf.Sin(Time.time * LimiterFrequency) + 1f) * LimiterSound * (speedRatio - LimiterEngage);
        }
        if (isEngineRunning)
        {
            idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
            if (speedSign > 0)
            {
                reverseSound.volume = 0;
                runningSound.volume = Mathf.Lerp(0.3f, runningMaxVolume, speedRatio);
                //runningSound.pitch = Mathf.Lerp(runningSound.pitch, Mathf.Lerp(0.3f, runningMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
                var runningVolumeProcent = Mathf.Lerp(runningSound.pitch, Mathf.Lerp(0.3f, runningMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
                runningSound.pitch = Mathf.Clamp(runningVolumeProcent, 0.5f, 1);
            }
            else
            {
                runningSound.volume = 0;
                reverseSound.volume = Mathf.Lerp(0f, reverseMaxVolume, speedRatio);
                //reverseSound.pitch = Mathf.Lerp(reverseSound.pitch, Mathf.Lerp(0.2f, reverseMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
                var runningVolumeProcent = Mathf.Lerp(runningSound.pitch, Mathf.Lerp(0.2f, runningMaxPitch, speedRatio) + revLimiter, Time.deltaTime);
                runningSound.pitch = Mathf.Clamp(runningVolumeProcent, 0.5f, 1);
            }
        }
        else {
            idleSound.volume = 0;
            runningSound.volume = 0;
        }
    }
    public IEnumerator StartEngine()
    {
        startingSound.Play();
        carController.isEngineRunning = 1;
        yield return new WaitForSeconds(0.6f);
        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);
        carController.isEngineRunning = 2;
    }

    public void StartSkid()
    {        
        if (Mathf.Abs(carController.wheelHits[0].sidewaysSlip) + Mathf.Abs(carController.wheelHits[0].forwardSlip) > 0.5f)
        {
            print(Mathf.Abs(carController.wheelHits[0].sidewaysSlip) + Mathf.Abs(carController.wheelHits[0].forwardSlip));
            if (!SlipSource.isPlaying)
            {
                SlipSource.Play();
                new WaitForSeconds(0.2f);
            }
            var slipVolumeProcent = Mathf.Abs(carController.steeringInput) * carController.steeringCurve.Evaluate(carController.speed);
            SlipSource.volume = slipVolumeProcent * 0.5f;
            SlipSource.pitch = Mathf.Clamp(slipVolumeProcent, 0.75f, 1);
        }
        else
        {
            SlipSource.Stop();
        }
    }

}
