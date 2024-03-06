using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantNoiseMaker : MonoBehaviour
{
    public float volume = 1.0f;
    public float distanceToHear;
    public AudioClip clip;
    public bool loopable = false;

    public AudioSource sourceFromSM;


    SoundManager sm;
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("New Audio");
        sm = GameObject.FindObjectOfType<SoundManager>();
        sourceFromSM = sm.newConstantAudio(volume, distanceToHear, clip, loopable, transform.position);
    }


}
