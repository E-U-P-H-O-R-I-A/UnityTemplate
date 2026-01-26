using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour
{
    public GameObject LaserEffects;
    public ParticleSystem LaserSparks;
    public ParticleSystem LaserSmoke;
    public AudioSource LaserChargeAudio;
    public AudioSource LaserAudio;
    public AudioSource LaserStopAudio;
    public GameObject LaserChargeBeam;
    public GameObject SmokeAndSparks;
    public GameObject ScorchMark;

    private GameObject ScorchMarkClone;
    private ParticleSystem.EmissionModule LaserSparksEmitter;
    private ParticleSystem.EmissionModule LaserSmokeEmitter;
    private int LaserChargeFlag = 0;

    void Start()
    {
        // Reset and stop all effects and audio

        LaserEffects.SetActive(false);

        LaserSparksEmitter = LaserSparks.emission;
        LaserSparksEmitter.enabled = false;

        LaserSmokeEmitter = LaserSmoke.emission;
        LaserSmokeEmitter.enabled = false;

        LaserChargeBeam.SetActive(false);
        SmokeAndSparks.SetActive(false);
        SmokeAndSparks.SetActive(true);

        ScorchMarkClone = Instantiate(ScorchMark);

        LaserChargeAudio.Stop();
        LaserAudio.Stop();
        LaserStopAudio.Stop();
    }

    void Update()
    {
        // Fire laser when left mouse button is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            LaserChargeFlag = 0;
            LaserChargeAudio.Play();
            LaserChargeBeam.SetActive(true);
            StartCoroutine(LaserChargeWait());
        }

        // Stop laser if left mouse button is released
        if (Input.GetButtonUp("Fire1"))
        {
            LaserChargeFlag = 1;
            LaserEffects.SetActive(false);
            LaserSparksEmitter.enabled = false;
            LaserSmokeEmitter.enabled = false;
            LaserAudio.Stop();
            LaserStopAudio.Play();
            LaserChargeBeam.SetActive(false);
        }
    }

    IEnumerator LaserChargeWait()
    {
        // Wait for laser to charge
        yield return new WaitForSeconds(1.4f);

        if (LaserChargeFlag == 0)
        {
            LaserEffects.SetActive(true);
            LaserSparksEmitter.enabled = true;
            LaserSmokeEmitter.enabled = true;
            LaserAudio.Play();
            ScorchMark.SetActive(true);
            LaserChargeFlag = 0;
        }
    }
}
