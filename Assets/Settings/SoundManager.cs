using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public List<AudioClip> musicClips = new List<AudioClip>();

    private AudioSource environment;
    private AudioSource playerAction;
    private SpriteRenderer playerSprite;
    private List<SpriteRenderer> waterList;
    private bool playerInWater = false;


    private float backGroundSwitchRate = 0.4f;
    private float backGroundVolume = 0.078f;
    private bool useFirstBackgroundSource = true;
    private AudioSource backgroundMusicSource1;
    private AudioSource backgroundMusicSource2;

    private Transform playerTransform; 
    
    void Start() {
        playerTransform = GameObject.FindObjectOfType<PlayerController>().transform;


        environment = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource1 = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource2 = gameObject.AddComponent<AudioSource>();
        playerAction = gameObject.AddComponent<AudioSource>();

        backgroundMusicSource1.loop = true;
        backgroundMusicSource2.loop = true;
        backgroundMusicSource2.volume = 0f;
        PlayRandomSound(musicClips, backgroundMusicSource1, 0.078f);
        // backgroundMusicSource.volume = 0.078f;

        SetUpWaterRegions();
    }

    private void SetUpWaterRegions() {
        playerSprite = GameObject.FindObjectOfType<PlayerGFX>().GetComponent<SpriteRenderer>();


        waterList = new List<SpriteRenderer>();
        GameObject[] waterObjects = GameObject.FindGameObjectsWithTag("Water");
        foreach (GameObject waterObject in waterObjects)
        {
            SpriteRenderer waterSprite = waterObject.GetComponent<SpriteRenderer>();
            if (waterSprite != null)
            {
                waterList.Add(waterSprite);
            }
            else
            {
                Debug.LogWarning("Water object '" + waterObject.name + "' does not have a SpriteRenderer component.");
            }
        }
    }

    public bool PlayerInWaterCheck() {
        float posX = playerSprite.bounds.center.x;
        float posY = playerSprite.bounds.min.y;

        // Log the names of the objects to the console
        foreach (SpriteRenderer waterSprite in waterList)
        {
            // Debug.Log("Object on Water layer: " + waterSprite.name);
            if (posX < waterSprite.bounds.max.x && posX > waterSprite.bounds.min.x &&
                posY < waterSprite.bounds.max.y && posY > waterSprite.bounds.min.y) {
                    return true;
                }
        }
        // return playerInWater;
        return false;
    }
    
    public void EnvironmentalSound(List<AudioClip> clipList, float volume) {
        environment.Stop();
        PlayRandomSound(clipList, environment, volume);
    }

    public void PlayerSound(List<AudioClip> clipList, float volume) {
        playerAction.Stop();
        PlayRandomSound(clipList, playerAction, volume);
    }

    public void SetNewBackgroundMusic(List<AudioClip> clipList, float volume) {
        backGroundVolume = volume;
        if (useFirstBackgroundSource) {
            PlayRandomSound(clipList, backgroundMusicSource1, volume);
        } else {
            PlayRandomSound(clipList, backgroundMusicSource2, volume);
        }
    }

    public void StopBackGroundMusic() {
        backGroundVolume = 0f;
        backgroundMusicSource1.volume = 0f;
        backgroundMusicSource2.volume = 0f;
    }


    private void RemoveMarkedForDestruction() {
        // lastingAudios.RemoveAll(audio => audio.canDestroy);

        List<ConstantAudios> tempCanKeep = new List<ConstantAudios>();

        if (lastingAudios.Count != 0) {
            for(int i = 0; i < lastingAudios.Count; i++) {
                if (!lastingAudios[i].canDestroy) {
                    tempCanKeep.Add(lastingAudios[i]);
                } else {
                    Destroy(lastingAudios[i].source);
                }
            }
        }

        lastingAudios = tempCanKeep;
    }

    private void EditConstantVolumes() {
        // lastingAudios.RemoveAll(audio => audio.canDestroy);

        if (lastingAudios.Count != 0) {
            for(int i = 0; i < lastingAudios.Count; i++) {

                float distance = Vector3.Distance(playerTransform.position, lastingAudios[i].soundPosition);
                float cappedPercentaged = Mathf.Min(1.0f, distance / lastingAudios[i].maxDistance);
                // Debug.Log("Capped percentaged = " + (1.0f - cappedPercentaged));
                lastingAudios[i].source.volume = lastingAudios[i].volume * (1.0f - cappedPercentaged);
                // lastingAudios[i].maxDistance
            }
        }

    }

    

    private void Update() {
        if (useFirstBackgroundSource) {
            backgroundMusicSource1.volume += backGroundSwitchRate * Time.deltaTime;
            backgroundMusicSource2.volume -= backGroundSwitchRate * Time.deltaTime;
            backgroundMusicSource1.volume = Mathf.Min(backgroundMusicSource1.volume, backGroundVolume);
        } else {
            backgroundMusicSource2.volume += backGroundSwitchRate * Time.deltaTime;
            backgroundMusicSource1.volume -= backGroundSwitchRate * Time.deltaTime;
            backgroundMusicSource2.volume = Mathf.Min(backgroundMusicSource1.volume, backGroundVolume);
        }

        // if (lastingAudios.Count != 0) {
        //     for(int i = 0; i < lastingAudios.Count; i++) {
        //         RemoveMarkedForDestruction
        //     }
        // }
        // RemoveMarkedForDestruction():
        RemoveMarkedForDestruction();
        EditConstantVolumes();

    }

    // Example method to play a random audio clip from the list
    public void PlayRandomSound(List<AudioClip> clipList, AudioSource audioSource, float volume)
    {
        if (clipList.Count > 0 && audioSource != null)
        {
            // Choose a random index
            int randomIndex = Random.Range(0, clipList.Count);

            // Get the selected AudioClip
            AudioClip selectedClip = clipList[randomIndex];

            // Assign the AudioClip to the AudioSource
            audioSource.clip = selectedClip;
            audioSource.volume = volume;

            // Play the AudioClip
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("sound list or AudioSource is not set up!");
        }
    }

    List<ConstantAudios> lastingAudios = new List<ConstantAudios>();

    // public void newConstantAudio(float volume, float distanceToHear, AudioSource source, AudioClip clip, bool loopable) {
    public AudioSource newConstantAudio(float volume, float distanceToHear, AudioClip clip, bool loopable, Vector3 soundPosition) {
        // Debug.Log("New Audio");
        AudioSource source = gameObject.AddComponent<AudioSource>();
        ConstantAudios newAudio = new ConstantAudios();
        newAudio.SetUp(volume, distanceToHear, source, clip, loopable, soundPosition);
        lastingAudios.Add(newAudio);
        source.loop = loopable;
        source.volume = volume;
        source.clip = clip;
        source.Play();
        if (!loopable) {
            float audioLength = clip.length;
            StartCoroutine(DelayedDestroy(newAudio, audioLength));
        }

        // Debug.Log("Got Called");
        return source;
    }

    private IEnumerator DelayedDestroy(ConstantAudios newAudio, float delay) {
        yield return new WaitForSeconds(delay);
       newAudio.canDestroy = true;
    }



    public class ConstantAudios {
        public float volume = 1f;
        public float maxDistance;
        public AudioSource source;
        public AudioClip clip;
        public bool willLoop = false;

        public bool canDestroy = false;
        public Vector3 soundPosition;

        public void SetUp(float v, float md, AudioSource s, AudioClip c, bool loopable, Vector3 sp) {
            volume = v;
            maxDistance = md;
            source = s;
            clip = c;
            willLoop =  loopable;
            soundPosition = sp;

        }


    }
}
