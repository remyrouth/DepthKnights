using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBlockTrapController : MonoBehaviour
{
    [Header("Object Variables")]
    public Transform bottomFallPoint;
    public Transform topRisePoint;
    public GameObject fallBlockStone;
    public SpriteRenderer fbRenderer;
    //distance between gameobject position and its
    // sprite renderer bounds
    private float centerSpriteRendererOffset;
    public SpriteRenderer glowTextRenderer;
    private UnityEngine.Rendering.Universal.Light2D light2D;
    private float maxIntensity; 

    [Header("Movement Variables")]
    public float initialDelay;

    public float fallRate;
    public float riseRate;
    public float RechargeRate;

    [Header("Sound Variables")]
    public float volume = 0.4f;
    public float distanceToHear = 0.5f;
    public AudioClip slamSound;
    public bool loopable = false;
    private SoundManager sm; 

    [Header("Damage Variables")]
    public Vector4 hitBoxOffset;
    public bool showBounds = false;
    private PlayerController pc;

    public List<Sprite> smashParticles = new List<Sprite>();
    public SpriteRenderer particleObject;
    private float particleCount;
    public float particleFrameRate = 0.2f;



    private void Start() {
        fbRenderer = fallBlockStone.GetComponent<SpriteRenderer>();
        light2D = glowTextRenderer.gameObject.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        maxIntensity = light2D.intensity; 

        Vector3 topBounds = new Vector3(fallBlockStone.transform.position.x,
            fallBlockStone.transform.position.y - fbRenderer.bounds.size.y / 2f,
            fallBlockStone.transform.position.z);

        centerSpriteRendererOffset = Vector3.Distance(fallBlockStone.transform.position,topBounds);
        Invoke("EnableTrapCycle", initialDelay);

        sm = FindObjectOfType<SoundManager>();
        pc = FindObjectOfType<PlayerController>();

        particleCount = smashParticles.Count;
    }

    private bool canMove = false;

    private void EnableTrapCycle() {
        canMove = true;
    }

    private void Update() {
        RiseAndFallController();
        Recharge();
        OnDrawGizmosSelected();
    }

    private bool canParticle = false;
    private int currentIndex = 0;

    private void SmashParticles() {
        if (!canParticle) {
            particleObject.gameObject.SetActive(false);
        } else {
            particleObject.gameObject.SetActive(true);

            particleObject.sprite = smashParticles[currentIndex];
            currentIndex++;
            
            if (currentIndex > particleCount) {
                currentIndex = 0;
                canParticle = false;
                SmashParticles();
            } else {
                Invoke("SmashParticles", particleFrameRate);
            }

        }
    }



    private float currentCharge = 100f;
    private void Recharge() {
        currentCharge += RechargeRate;
        currentCharge = Mathf.Min(currentCharge, 100f);

        float percentage = currentCharge / 100f;
        float alpha = 255f * percentage;
        float alphaNormalized = alpha / 255f;

        light2D.intensity = maxIntensity/percentage;

        if (glowTextRenderer != null) {
            Color originalColor = glowTextRenderer.color;
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alphaNormalized);
            glowTextRenderer.color = newColor;
        }
        // Debug.Log(alphaPercentage);
    }

    private void CreateSound() {
        sm.newConstantAudio(volume, distanceToHear, slamSound,
            loopable, transform.position);
    }

    private void DamageCheck() {
        if (IsPositionWithinBounds(pc.gameObject.transform.position, fbRenderer)) {
            pc.DecreaseHealth(1f);
        }
    }

    // Method to check if a position is within the bounds of a SpriteRenderer
    public bool IsPositionWithinBounds(Vector3 position, SpriteRenderer spriteRenderer)
    {
        // Get the bounds of the SpriteRenderer
        Bounds bounds = spriteRenderer.bounds;

        // Horizontal
        float minAddativeX = hitBoxOffset.x;
        float maxSubtractiveY = hitBoxOffset.y;

        // Vertical
        float minAddativeZ = hitBoxOffset.z;
        float maxSubtractiveW = hitBoxOffset.w;

        // Check if the position is within the bounds
        if (position.x >= bounds.min.x + minAddativeX && position.x <= bounds.max.x -  maxSubtractiveY
        && position.y >= bounds.min.y + minAddativeZ && position.y <= bounds.max.y - maxSubtractiveW) 
        // && .z >= bounds.min.z && position.z <= bounds.max.z)
        {
            Debug.Log("MinX is " + bounds.min.x);
            Debug.Log("MaxX is " + bounds.max.x);
            Debug.Log("PlayerPosX is " + position.x);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (showBounds) {
            DrawBoundsGizmo();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showBounds) {
            DrawBoundsGizmo();
        }

    }

    private void DrawBoundsGizmo()
    {
        // if (fbRenderer == null)
        //     return;

        // Horizontal
        float minAddativeX = hitBoxOffset.x;
        float maxSubtractiveY = hitBoxOffset.y;

        // Vertical
        float minAddativeZ = hitBoxOffset.z;
        float maxSubtractiveW = hitBoxOffset.w;

        Bounds bounds = fbRenderer.bounds;
        // Debug.Log(bounds);
        Vector3 boundsCenter = bounds.center;
        Vector3 boundsExtents = bounds.extents;

        // Calculate the corners of the bounds
        Vector3 topLeft = boundsCenter + new Vector3(-boundsExtents.x + minAddativeX, boundsExtents.y - maxSubtractiveW, 0);
        Vector3 topRight = boundsCenter + new Vector3(boundsExtents.x - maxSubtractiveY, boundsExtents.y - maxSubtractiveW, 0);
        Vector3 bottomLeft = boundsCenter + new Vector3(-boundsExtents.x + minAddativeX, -boundsExtents.y + minAddativeZ, 0);
        Vector3 bottomRight = boundsCenter + new Vector3(boundsExtents.x - maxSubtractiveY, -boundsExtents.y + minAddativeZ, 0);


        // // Calculate the corners of the bounds
        // Vector3 topLeft = boundsCenter + new Vector3(-boundsExtents.x, boundsExtents.y, 0);
        // Vector3 topRight = boundsCenter + new Vector3(boundsExtents.x, boundsExtents.y, 0);
        // Vector3 bottomLeft = boundsCenter + new Vector3(-boundsExtents.x, -boundsExtents.y, 0);
        // Vector3 bottomRight = boundsCenter + new Vector3(boundsExtents.x, -boundsExtents.y, 0);

        // Draw lines between the corners to outline the bounds
        Gizmos.color = Color.red;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }



    private bool fallingCycle = true;
    private void RiseAndFallController()
    {
        if (!canMove)
        {
            return;
        }

        if (fallingCycle)
        {
            float newYPos = fallBlockStone.transform.position.y - fallRate;
            newYPos = Mathf.Max(newYPos, bottomFallPoint.position.y - centerSpriteRendererOffset);
            fallBlockStone.transform.position = new Vector3(fallBlockStone.transform.position.x, newYPos, fallBlockStone.transform.position.z);

            if (fallBlockStone.transform.position.y <= bottomFallPoint.position.y - centerSpriteRendererOffset)
            {
                fallingCycle = false; // Because now we should start rising
                currentCharge = 0f;
                CreateSound();
                DamageCheck();

                currentIndex = 0;
                canParticle = true;
                SmashParticles();
            }
        }
        else
        {
            float newYPos = fallBlockStone.transform.position.y + riseRate;
            newYPos = Mathf.Min(newYPos, topRisePoint.position.y + centerSpriteRendererOffset);
            fallBlockStone.transform.position = new Vector3(fallBlockStone.transform.position.x, newYPos, fallBlockStone.transform.position.z);

            if (fallBlockStone.transform.position.y >= topRisePoint.position.y + centerSpriteRendererOffset
             && currentCharge == 100f)
            {
                fallingCycle = true; // Because now we should start falling
            }
        }
    }


}
