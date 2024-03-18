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
    public string gfxChildName = "gfx";

    public float health = 3f;
    public float takingDamageTimer = 0.3f;
    public float speed;
    public float attackingTimer = 0.3f;
    public float idleAfterAttackTimer = 0.3f;
    public float attackRange;

    public Slider healthBar;
    public Capabilities entityCapabilities;
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



    // Start is called before the first frame update
    void Start()
    {
        animator = transform.Find(gfxChildName).GetComponent<Animator>();
        egfx = transform.Find(gfxChildName).GetComponent<EnemyGFX>();
        maxHealth = health;
        sm = FindObjectOfType<SoundManager>();
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

            default:
                // Handle any unexpected state
                // Debug.LogWarning("Unhandled player state");
                break;
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

    private void MoveFoward() {
        if (!isAttacking) {
            Debug.Log("Moving Forward");
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
        // if (hitForward.collider.gameObject.tag == "Player") {
        //     hitForward.collider.gameObject.GetComponent<PlayerController>().DecreaseHealth(10f);
        //     Debug.Log("Found player");
        // } else {
        //     Debug.Log("Attacked with damage but did not find player");
        // }
        if (hitForward.collider != null) {
            Debug.Log("Found player");
            hitForward.collider.gameObject.GetComponent<PlayerController>().DecreaseHealth(1f);
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
