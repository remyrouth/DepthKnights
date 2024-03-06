using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneTrigger : MonoBehaviour
{
    public GameObject enemyTrigger;
    public GameObject bossObject;
    private Collider2D collider;

    public AnimationCurve speedStopCurve;
    private float maxDistance;
    public float maxSpeed = 5f;

    private bool isActivated = false;

    public float aniTriggerRange = 3f;
    private bool activatedAniYet = false;
    public Animator bossAnimator;




    public float flashRangeMax = 0.5f;
    public float flashRangeMin = 0.1f;
    private bool CanToggle = true;
    public float toggleLength = 10f;

    public GameObject regGrid;
    public GameObject bossGrid;

    // Sounds 
    private SoundManager sm;
    public float phaseChangeVolume = 0.8f;
    public List<AudioClip> phaseChangeClips = new List<AudioClip>();

    void Start()
    {
        collider = enemyTrigger.GetComponent<Collider2D>();
        maxDistance = Vector3.Distance(bossObject.transform.position, transform.position);
        sm = FindObjectOfType<SoundManager>();
        
    }

    private void ToggleGrids() {
        if (!CanToggle) {
            return;
        }
        sm.EnvironmentalSound(phaseChangeClips, phaseChangeVolume);
        regGrid.SetActive(!regGrid.activeSelf);
        bossGrid.SetActive(!bossGrid.activeSelf);

        float randomValue = Random.Range(flashRangeMax, flashRangeMin);
        Invoke("ToggleGrids", randomValue);
    }

    private void StopToggle() {
        sm.EnvironmentalSound(phaseChangeClips, phaseChangeVolume);
        CanToggle = false;
        regGrid.SetActive(false);
        bossGrid.SetActive(true);
    }

    public float endSceneTimer = 3f;

    public float musicVolume = 0.8f;
    public List<AudioClip> MusicChangeClips = new List<AudioClip>();

    public float bossVolume = 0.8f;
    public List<AudioClip> endSceneSFXlist = new List<AudioClip>();
    public float nextSceneTimer = 3f;

    private void EndTutorial() {
        CameraToPlayer ctp = Camera.main.GetComponent<CameraToPlayer>();
        ctp.StartTransition(100f);
        sm.SetNewBackgroundMusic(endSceneSFXlist, bossVolume);
        Invoke("NextScene", nextSceneTimer);
    }

    private void NextScene() {
        // string inputSceneName = sceneNameInput.text;
       SceneManager.LoadScene("MainMenu");
    }



    void Update()
    {
        if (!collider.enabled && !isActivated)
        {
            // Ensure isActivated is false before setting it to true
            if (!isActivated)
            {
                PlayerController pc = FindObjectOfType<PlayerController>();
                pc.ChangePlayStatus(false);

                sm.SetNewBackgroundMusic(MusicChangeClips, musicVolume);
                isActivated = true;
                ToggleGrids();
                Invoke("StopToggle", toggleLength);
                Invoke("EndTutorial", endSceneTimer);
            }
        }

        // Debug.Log("distancePercentage: " + distancePercentage);
        if (isActivated)
        {
            float distancePercentage = Vector3.Distance(bossObject.transform.position, transform.position) / maxDistance;
            // float distancePercentage = Vector3.Distance(bossObject.transform.position, transform.position) / maxDistance;
            float curveSpeed = 1f - speedStopCurve.Evaluate(distancePercentage);
            bossObject.transform.Translate(Vector3.up * maxSpeed * curveSpeed * Time.deltaTime);
        }

        if (!activatedAniYet) {
            float distance = Vector3.Distance(bossObject.transform.position, transform.position);
            // Debug.Log("Distance: " + distance + "___ " + aniTriggerRange);
            if (distance < aniTriggerRange) {
                activatedAniYet = true;
                bossAnimator.SetInteger("AnimState", 1);
                // Debug.Log("Triggered animation");
            }

        }
    }
}
