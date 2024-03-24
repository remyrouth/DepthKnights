using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    // [Tooltip("Reference to the Animator component")]
    [SerializeField]
    public enum Capabilities {
        BrainlessWander, // cannot jump
        Intelligent,
        Dummy, // has no action
        FlyableIntelligent
    }
    [Header("Behavior Variables")]
    public string gfxChildName = "gfx";
    public float attentionSpan = 4f;
    public float detectionRange = 6f;
    public Capabilities entityCapabilities;
    private bool isAgro = false;

    [Header("Stats Variables")]
    public Slider healthBar;
    public float health = 3f;
    public float takingDamageTimer = 0.3f;
    public float speed;

    [Header("Attacking Variables")]
    public float attackingTimer = 0.3f;
    public float idleAfterAttackTimer = 0.3f;
    public float attackRange;
    public float smackForce = 1f;

    [Header("Sound Variables")]
    public float damagedSoundVolume = 0.8f;
    public AudioClip damagedSound;

    // Basic Ai concept
    // if player is within a specified range and is in direction npc is facing
    // start raycasting in said direction. If line of sight, use A* to get on same level
    // when on same level, move towards enemy
    // (HOLLOW KNIGHT does not use A* unless its for flying enemies)



    // State bools
    private bool isTakingDamage = false;
    private bool isAttacking = false;
    private bool facingLeft = true;
    private bool isDead = false;





    private float maxHealth;
    private Animator animator;
    private EnemyGFX egfx;
    private SoundManager sm;
    private PlayerController pc;
    private Timer timer;



    // Start is called before the first frame update
    void Start()
    {
        animator = transform.Find(gfxChildName).GetComponent<Animator>();
        egfx = transform.Find(gfxChildName).GetComponent<EnemyGFX>();
        maxHealth = health;
        sm = FindObjectOfType<SoundManager>();
        pc = FindObjectOfType<PlayerController>();

        if (entityCapabilities != Capabilities.BrainlessWander) {
            timer = gameObject.AddComponent<Timer>();
        }
    }

    void Update() {
        if (isDead) {
            return;
        }
        DirectionControl();

        SetEnemyState(entityCapabilities);
    }

    void SetEnemyState(Capabilities newState) {
        switch (newState)
        {
            case Capabilities.BrainlessWander:
                // Handle BrainlessWander state
                SlimeState();
                break;

            case Capabilities.Intelligent:
                // Handle BrainlessWander state
                GoblinState();
                break;

            default:
                // Handle any unexpected state
                // Debug.LogWarning("Unhandled player state");
                break;
        }
    }

    private void GoblinState() {
        // Debug.Log("isTakingDamage: " + isTakingDamage);
        if (isTakingDamage || isAttacking || isDead) {
            if (isDead) {
                egfx.TriggerDeathAnim();
            }
            return;
        }

        // Debug.Log("AgroState: " + isAgro + " |    time: " + timer.GetTime() + "     attention span is: " + attentionSpan);

        egfx.TriggerMoveAnim();
        if (CanMoveForward()) {
            // Debug.Log("Moving Forward");
            DetectPlayer();
            AttackForward();
            MoveFoward();
        } else {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    private void DetectPlayer() {
        if (timer.GetTime() > attentionSpan) {
            isAgro = false;
        } else {
            isAgro = true;
        }
        isPlayerInFront();

    }

    private void isPlayerInFront() {
        Vector2 forwardDirection = Vector2.right;
        if (facingLeft) {
            forwardDirection = Vector2.left;
        }
        int groundLayerMask = LayerMask.GetMask("Player", "Ground");
        RaycastHit2D forward = Physics2D.Raycast(transform.position, forwardDirection, detectionRange, groundLayerMask);

        // RaycastHit2D forward = Physics2D.Raycast(transform.position, forwardDirection, detectionRange);
        Debug.DrawRay(transform.position, forwardDirection * detectionRange, Color.red);

        bool didDetect = (forward.collider != null); // if its not null, it means a player was detected
        bool hitPlayer = false;

        if (didDetect) {
            string hitTag = forward.collider.tag;
            if (hitTag == "Player") {
                hitPlayer = true;
            }
        }

        if (hitPlayer) {
            Debug.Log("ENEMY RAYCAST HAS HIT PLAYER");
            timer.ResetTimer();
        }
    }

    private void SlimeState() {
        // Debug.Log("isTakingDamage: " + isTakingDamage);
        if (isTakingDamage || isAttacking || isDead) {
            if (isDead) {
                egfx.TriggerDeathAnim();
            }
            return;
        }

        egfx.TriggerMoveAnim();
        if (CanMoveForward()) {
            // Debug.Log("Moving Forward");
            AttackForward();
            MoveFoward();
        } else {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    // if we're chasing the player (this enemy is agrovated/agro), we should stop in front of the player
    private bool playerInStopRange() {
        float playerX = pc.gameObject.transform.position.x;
        float selfX = transform.position.x;


        // we should also turn around if they're on the other side and we're still agro
        float sideTell = playerX - selfX;
        if (sideTell > 0f) {
            // means player is to the right, due to having a greater x
            if (facingLeft) {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                facingLeft = !facingLeft;
            }
        } else {
            // means player is to the left, due to having a smaller x
            if (!facingLeft) {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                facingLeft = !facingLeft;
            }
        }


        if (facingLeft) {
            float coordDiff = playerX - selfX;
            // player is on left, so smaller x... and selfx is larger
            // so coord diff will be negative but greater than -0.2f
            bool inStopRange = coordDiff < 0f && coordDiff > -0.4f;
            return inStopRange;
        } else {
            float coordDiff = playerX - selfX;
            // player is on right, so bigger x... and selfx is smaller
            // so coord diff will be positive but less than 0.2f
            bool inStopRange = coordDiff > 0f && coordDiff < 0.4f;
            return inStopRange;
        }

        // transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

    }

    private void MoveFoward() {
        if (isAgro) {
            if (playerInStopRange()) {
                return;
            }
        }

        if (!isAttacking) {
            // Debug.Log("Moving Forward");
            if (facingLeft) {
                transform.position = new Vector3(transform.position.x - (speed * Time.deltaTime), transform.position.y, transform.position.z); 
            } else {
                transform.position = new Vector3(transform.position.x + (speed * Time.deltaTime), transform.position.y, transform.position.z); 
            }
        }
    }

    private void AttackForward() {
        Vector2 forwardDirection = Vector2.right;
        if (facingLeft) {
            forwardDirection = Vector2.left;
        }

        int groundLayerMask = LayerMask.GetMask("Player");

        // Raycast forward
        RaycastHit2D hitForward = Physics2D.Raycast(transform.position, forwardDirection, attackRange, groundLayerMask);
        Debug.DrawRay(transform.position, forwardDirection * 5f, Color.green);

        bool canAttack = (hitForward.collider != null); // should not be null

        if (canAttack && !isAttacking) {
            isAttacking = true;
            egfx.TriggerAttackAnim();
            Invoke("ResetAfterAttacking", idleAfterAttackTimer);
            Invoke("EndAttackState", attackingTimer);
            // Debug.Log("Started Attack");
        }
    }

    private void ResetAfterAttacking(){
        egfx.SetAnimationState(0);
    }

    public void EnemyDealsDamageOnFrame() { // for attacking, called by enemy gfx script 
        int groundLayerMask = LayerMask.GetMask("Player");
        Vector2 forwardDirection = Vector2.right;
        if (facingLeft) {
            forwardDirection = Vector2.left;
        }
        RaycastHit2D hitForward = Physics2D.Raycast(transform.position, forwardDirection, attackRange, groundLayerMask);
        if (hitForward.collider != null) {
            Debug.Log("Found player");
            hitForward.collider.gameObject.GetComponent<PlayerController>().DecreaseHealth(1f);
            Rigidbody2D rb = hitForward.collider.gameObject.GetComponent<Rigidbody2D>();
            Vector2 directionAway = (Vector2)transform.position - rb.position;
            directionAway.Normalize();
            rb.AddForce(directionAway * -1 * smackForce, ForceMode2D.Impulse);
        } else {
            Debug.Log("Did not find player");
        }
    }

    public void EndAttackState() {
        isAttacking = false;
        egfx.SetAnimationState(0);
        // Debug.Log("Ended Attack");
    }

    private bool CanMoveForward() {
        Vector2 forwardDirection = Vector2.right;
        if (facingLeft) {
            forwardDirection = Vector2.left;
        }

        int groundLayerMask = LayerMask.GetMask("Ground", "Breakable");

        // Raycast to the right
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, forwardDirection, attackRange, groundLayerMask);
        Debug.DrawRay(transform.position, forwardDirection * 5f, Color.green);

        // Raycast to the right downward at a 45-degree angle
        float angle = facingLeft ? -315 : 315; // Adjust the angle based on facing direction
        Vector2 direction = Quaternion.Euler(0, 0, angle) * forwardDirection;
        RaycastHit2D hitRightDown = Physics2D.Raycast(transform.position, direction, attackRange, groundLayerMask);
        Debug.DrawRay(transform.position, direction * 5f, Color.blue);


        // Check if the ray hits something
        bool hasSomethingToWalkOn = (hitRightDown.collider != null); // should not be null
        bool hasSpaceToWalk = (hitRight.collider == null); // should be null


        return hasSomethingToWalkOn && hasSpaceToWalk;

    }

    public void DecreaseHealth() {
        if (isTakingDamage) {
            return;
        }

        if (entityCapabilities != Capabilities.BrainlessWander) {
            // face player
            isAgro = true;
            timer.ResetTimer();
        }

        // Debug.Log("Decreased Health");
        health--;
        health = Mathf.Max(0, health);
        healthBar.value = (health/maxHealth);

        // Sound Details
        List<AudioClip> list = new List<AudioClip>();
        list.Add(damagedSound);
        sm.PlayerSound(list, damagedSoundVolume);

        // Anim Details
        isTakingDamage = true;
        isAttacking = false;
        egfx.SetAnimationState(2);

        Invoke("EndDamagedState", takingDamageTimer);

        if (health == 0) {
            StartDeath();
        }

    }
    public void EndDamagedState() {
        isTakingDamage = false;
    }

    private void StartDeath() {
        Debug.Log("Death Started");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
        // rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        GetComponent<Collider2D>().enabled = false;
        healthBar.gameObject.SetActive(false);
        egfx.SetAnimationState(1);
        isDead = true;
    }

    private void DirectionControl() {
        if (transform.localScale.x > 0) {
            facingLeft = true;
        } else {
            facingLeft = false;
        }
    }

}
