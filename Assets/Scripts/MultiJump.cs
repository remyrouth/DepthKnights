using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiJump : MonoBehaviour
{
    private PlayerController pc;
    private Rigidbody2D rb;
    [Header("Jump Variables")]
    public int extraJumpCount = 1;


    // if you air jump immediatly after jumping from the ground, you get a super jump... 
    // consider limiting the max upward velocity, or adding a timer that will enable 
    // air jumps after a ground jump
    public float canJumpAgainRate = 0.5f;
    public float offGroundAirJumpDelay = 0.4f;
    public float jumpForce = 10f;
    public float maxUpwardVelocity = 20f; // Maximum upward velocity
    private int currentJumpCountLeft;
    // Start is called before the first frame update

    [Header("Particle Variables")]
    public float VFXFrameRate = 0.15f;
    public float opacityFadeRate = 0.5f;
    public List<Sprite> particleList = new List<Sprite>();
    public float yOffset = 0.5f;
    private List<Vector3> jumpLocList = new List<Vector3>();
    public List<GameObject> spawnList = new List<GameObject>();



    private float startTime;
    private float timeSinceNotGrounded;


    [Header("Sound Variables")]
    public float volume = 0.5f;
    public float distanceToHear = 3f;
    public AudioClip AbilitySound;
    private bool loopable = false;
    private SoundManager sm;

    void Start()
    {
        currentJumpCountLeft = extraJumpCount;
        pc = FindObjectOfType<PlayerController>();
        // jumpForce = pc.jumpForce;
        rb = GetComponent<Rigidbody2D>();

        startTime = Time.time;
        sm = FindObjectOfType<SoundManager>();


        UpdateObjectSpritesUsed();
    }


    private void UpdateOpacityObjects()
    {
        List<GameObject> newSpawnList = new List<GameObject>();

        for (int i = 0; i < spawnList.Count; i++)
        {
            GameObject vault = spawnList[i];
            if (vault != null)
            {
                SpriteRenderer sr = vault.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    float alpha = c.a - opacityFadeRate;
                    alpha = Mathf.Clamp(alpha, 0f, 1f);
                    Color newColor = new Color(c.r, c.g, c.b, alpha);
                    sr.color = newColor;

                    if (alpha > 0f)
                    {
                        newSpawnList.Add(vault);
                    }
                    else
                    {
                        Destroy(vault);
                    }
                }
                else
                {
                    Debug.Log("No renderer found");
                }
            }
        }

        spawnList.Clear();
        spawnList.AddRange(newSpawnList);
    }

    private void UpdateObjectSpritesUsed() {
        // Debug.Log("UpdateObjectSpritesUsed called");
        for (int i = 0; i < spawnList.Count; i++) {
            GameObject vault = spawnList[i];
            if (vault != null) {
                SpriteRenderer sr = vault.GetComponent<SpriteRenderer>();
                if (sr != null) {
                    int index = FindMatchingSpriteIndex(sr.sprite, particleList);
                    // Debug.Log("Index: " + index);

                    if (index >= particleList.Count - 1 || index == -1) {
                        sr.sprite = particleList[0];
                    } else {
                        
                        sr.sprite = particleList[index + 1];
                        // Debug.Log("Index Chosen: " + index + 1);
                    }
                } else {
                    Debug.Log("No renderer found");
                }
            }
        }

        Invoke("UpdateObjectSpritesUsed", VFXFrameRate);
    }

    public int FindMatchingSpriteIndex(Sprite spriteToCheck, List<Sprite> spriteList) {
        for (int i = 0; i < spriteList.Count; i++) {
            if (spriteList[i] == spriteToCheck) {
                return i; // Return the index of the matching sprite
            }
        }
        // If no matching sprite is found, return -1
        Debug.Log("Returned -1");
        return -1;
    }


    private void Update() {
        if (pc.IsGrounded()) {
            currentJumpCountLeft = extraJumpCount;
            startTime = Time.time;
        } else {
            timeSinceNotGrounded = Time.time - startTime;   
        }

        // Debug.Log("timeSinceNotGrounded: " + timeSinceNotGrounded + "       startTime: " + startTime);


        UpdateOpacityObjects();
        // UpdateObjectSpritesUsed();
    }

    private bool canAirJump = true;

    public void TriggerJump(){
        // if (currentJumpCountLeft > 0 && canAirJump) {
        if (currentJumpCountLeft > 0 && timeSinceNotGrounded >= offGroundAirJumpDelay && canAirJump) {
            
            // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // currentJumpCountLeft--;
            // Check current velocity


            if (rb.velocity.y > maxUpwardVelocity)
            {
                // If current velocity exceeds the maximum, limit it
                rb.velocity = new Vector2(rb.velocity.x, maxUpwardVelocity);
            }
            else
            {
                // Otherwise, add the jump force
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            currentJumpCountLeft--;


            
            jumpLocList.Add(transform.position);
            GameObject spawnedObject = new GameObject("SpawnedObject");
            spawnList.Add(spawnedObject);
            spawnedObject.transform.position = new Vector3(transform.position.x, transform.position.y - yOffset, 0);
            SpriteRenderer spriteRenderer = spawnedObject.AddComponent<SpriteRenderer>();
            // spriteRenderer.sprite = sprite;
            spriteRenderer.sprite = particleList[0];
            spriteRenderer.sortingLayerName = "Ground";
            PlaySound();

            // Set the order in layer to 10
            spriteRenderer.sortingOrder = 10;

            canAirJump = false;
            Invoke("EnableJumpAbility", canJumpAgainRate);
        }
    }

    private void PlaySound() {
        sm.newConstantAudio(volume, distanceToHear, AbilitySound,
            loopable, transform.position);
    }

    private void EnableJumpAbility() {
        canAirJump = true;
    }
}
