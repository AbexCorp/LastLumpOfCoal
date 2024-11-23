using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;


    [SerializeField]
    private AudioMixer _audioMixer;

    [SerializeField]
    private AudioSource _doorSlam;
    [SerializeField]
    private AudioSource _whistle;
    [SerializeField]
    private AudioSource _stopSteam;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Train.Instance.OnDoorOpen += TrainOpen;
        Train.Instance.OnDoorClose += TrainClose;


        _audioMixer.SetFloat("WindMuff", 1000);


        lastPlayTime = -Mathf.Infinity; // Ensure it starts ready to play
    }


    private void TrainOpen()
    {
        _audioMixer.SetFloat("WindMuff", 22000);
        _doorSlam.time = 0.35f;
        _doorSlam.Play();
    }
    private void TrainClose()
    {
        _audioMixer.SetFloat("WindMuff", 1000);
        _doorSlam.time = 0.35f;
        _doorSlam.Play();
    }
    public void Whistle()
    {
        _whistle.Play();
    }
    public void Steam()
    {
        _stopSteam.Play();
    }











    [Header("Audio Settings")]
    public Rigidbody2D TrainRB;
    public AudioSource[] audioSource; // AudioSource component

    [Header("Velocity Settings")]
    public float speed1 = 10f; // Lower speed threshold
    public float speed2 = 20f; // Upper speed threshold
    public float delay1 = 1f; // Delay for speed1
    public float delay2 = 0.5f; // Delay for speed2

    [Header("Debug")]
    public float currentVelocity; // Current velocity for debugging

    private float lastPlayTime; // Last time a clip was played


    float prevVelocity = 0;
    void Update()
    {
        currentVelocity = GetVelocity(); // Replace with your actual velocity logic
        if (currentVelocity < speed1)
            return;
        float delay = CalculateDelay();

        if (Time.time >= lastPlayTime + delay)
        {
            PlayRandomClip();
            lastPlayTime = Time.time;
        }
    }

    float GetVelocity()
    {
        currentVelocity = TrainRB.velocity.x;
        return currentVelocity;
    }

    float CalculateDelay()
    {
        if (currentVelocity <= speed1)
        {
            return delay1;
        }
        else if (currentVelocity >= speed2)
        {
            return delay2;
        }
        else
        {
            // Interpolate delay between delay1 and delay2 based on currentVelocity
            float t = (currentVelocity - speed1) / (speed2 - speed1);
            return Mathf.Lerp(delay1, delay2, t);
        }
    }

    int previousrng = -1;
    void PlayRandomClip()
    {
        if (audioSource.Length == 0)
            return;

        int randomIndex = Random.Range(0, audioSource.Length);
        while(randomIndex == previousrng)
        {
            randomIndex = Random.Range(0, audioSource.Length);
        }
        previousrng = randomIndex;
        audioSource[randomIndex].PlayOneShot(audioSource[randomIndex].clip);
    }




}