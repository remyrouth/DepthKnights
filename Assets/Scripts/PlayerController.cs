using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Variables")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 10f;
    public float dashTimer = 1f;

    [Header("Health Variables")]
    private float health = 10;
    private float maxHealth;
    public Slider healthBar;

    [Header("Stamina Variables")]
    private float stamina = 100f;
    public Slider staminaBar;
    public float staminaRechargeRate = 0.5f;
    public float dashCost = 15f;
    public float wallHangStaminaLossRate = 0.5f;

    [Header("Wall Jump Variables")]
    public float wallHangJumpCost = 10f;
    public float wallJumpSideForce = 5f;

    private bool canDash = true;

    // Components
    private Rigidbody2D rb;
    private  Collider2D collider;

    private PlayerGFX gfx;
    private GameManager gm;

    // Limiters
    [Header("Raycast Limiters")]
    public float sideMoveBoundary = 0.5f;
    public float groundCheckDistance = 0.2f;

    // Status variables
    private bool isCrouched = false;
    private bool isDashing = false;
    private float ADCheck = 0f;
    private GameObject wallhangChild;
    private bool jumpButtonsHeldDown = false;
    private bool isAttackingCurrently = false;


    // Attacking Variables
    [Header("Attacking Variables")]
    private float damage = 1f;
    public float attackRange = 0.8f;
    private bool isInWater = false;


    // Being Damaged Variables
    [Tooltip("Being Damaged Variables")]
    public float damageStatusLength = 0.5f;
    private bool isBeingDamaged = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        wallhangChild = transform.Find("WallHangParent").gameObject;
        gfx = transform.Find("WallHangParent").gameObject.transform.Find("WarriorGFX").GetComponent<PlayerGFX>();
        gm = FindObjectOfType<GameManager>();
        gm.InitializeEden(this);

        // health = Mathf.Max(0, health);
        // health--;
        // health = Mathf.Max(0, health);
        // healthBar.value = (health/maxHealth);
        // healthBar.value = (health/maxHealth);
        maxHealth = health;

    }

    void Update()
    {
        HandleInput();
        HandleScriptedInput();

        // recharging here
        stamina += staminaRechargeRate * Time.deltaTime;
        stamina = Mathf.Min(stamina, 100f);
        staminaBar.value = stamina/100f;

        // Debug.Log("Stamina: " + stamina + "     isDashing: " + isDashing);
    }

    public void DecreaseHealth(float damageToTake) {
        if (!isBeingDamaged &&  !isDashing) {

            // health--;
            health -= damageToTake;
            health = Mathf.Max(0, health);
            healthBar.value = (health/maxHealth);


            if (health == 0) {
                isDead = true;
            } else {
                isBeingDamaged = true;
            }
            // Invoke("EndBeingDamagedStatus", damageStatusLength);
        }
    }

    public void EndBeingDamagedStatus() {
        isBeingDamaged = false;
    }


    // called by GameManager
    public void StartEdenAttackType1() {
        if (!canPlay) {
            return;
        }
        isAttackingCurrently = true;
    }

    public void EndEdenAttackType1() {
        isAttackingCurrently = false;
    }

    public bool Attack1Check() {
        return isAttackingCurrently;
    }

    private bool damageActivated = false;

    public void DealDamageOnTriggerAttackFrame() {
        Vector2 direction = Vector2.right;
        if (transform.localScale.x < 0) {
            direction = Vector2.left;
        } 

        // Debug.Log("damageActivated changed to true, was orignally " + damageActivated);
        RaycastHit2D attackHit = Physics2D.Raycast(transform.position, direction, attackRange, LayerMask.GetMask("Breakable", "Enemy", "Interactable"));

        if (attackHit.collider != null) {
            BreakableController bc = attackHit.collider.gameObject.GetComponent<BreakableController>();
            if (bc != null) {
                Debug.Log("Breakable hit");
                bc.DecreaseHealth();
                return;
            }
            // bc.DecreaseHealth();

            EnemyController ec = attackHit.collider.gameObject.GetComponent<EnemyController>();
            if (ec != null) {
                Debug.Log("Enemy hit");
                ec.DecreaseHealth();
                return;
            }

            LeverActivator la = attackHit.collider.gameObject.GetComponent<LeverActivator>();
            if (la != null) {
                Debug.Log("Lever hit");
                la.ToggleSwitch();
                return;
            }

            Debug.Log("called on nothing");
        }
        // damageActivated = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Check if the mouse button is clicked
        if (damageActivated)
        {
           BreakableController bc = other.gameObject.GetComponent<BreakableController>();
            if (bc != null) {
                bc.DecreaseHealth();
            }
            // Reset the flag after processing the click
            damageActivated = false;
            Debug.Log("damageActivated changed back to false");
        }
    }

    private bool scriptAutoWalkRight = false;
    private bool scriptAutoWalkLeft = false;

    public void AutoWalkRight(bool status) {
        scriptAutoWalkRight = status;
    }

   public void AutoWalkLeft(bool status) {
        scriptAutoWalkLeft = true;
    }

    private bool canPlay = true;

    public void ChangePlayStatus(bool status) {
        canPlay = status;
    }

    private void HandleScriptedInput() {
        if (canPlay || isDead) {
            return;
        }

        // rb.constraints =  RigidbodyConstraints2D.FreezeRotation;
        if (scriptAutoWalkRight)
        {
            Debug.Log("walking right");
            Debug.Log("Walked right" + scriptAutoWalkRight);
            MoveLeftRight(Vector2.right);
            ADCheck = -1f;
        } 
        else if (scriptAutoWalkLeft) {
            MoveLeftRight(-1 * Vector2.right);
            ADCheck = 1f;
        } else {
            ADCheck = 0f;
        }
    }

    private void HandleInput() {
        if (!canPlay || isDead) {
            return;
        }

        if (isBeingDamaged) {
            return;
        }

        // rb.constraints =  RigidbodyConstraints2D.FreezeRotation;
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeftRight(-1 * Vector2.right);
            ADCheck = -1f;
        } 
        else if (Input.GetKey(KeyCode.D)) {
            MoveLeftRight(Vector2.right);
            ADCheck = 1f;
        } else {
            ADCheck = 0f;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();

            if (WallHangCheck() && !isAttackingCurrently) {
                if (transform.localScale.x < 0) { // facing right
                    rb.AddForce(Vector2.right * wallJumpSideForce, ForceMode2D.Impulse);
                } else { // facing left
                    rb.AddForce(Vector2.left * wallJumpSideForce, ForceMode2D.Impulse);
                }
                // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            }
            jumpButtonsHeldDown = true;
        } 
        
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space)) {
            jumpButtonsHeldDown = false;
        } 

        // Debug.Log("isCrouched: " + isCrouched + "    IsGrounded: " + IsGrounded());

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        wallhangChild.transform.localScale = new Vector3(Mathf.Abs(wallhangChild.transform.localScale.x),
                                    wallhangChild.transform.localScale.y, wallhangChild.transform.localScale.z); 
        // Check if the Control key is pressed
        // if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        if (ADCheck == 0f && !IsGrounded())
        {
            CrouchAndHang();
        } else {
            isCrouched = false;
            // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Dash();
        }

    }


    private void CrouchAndHang() {
        if (IsGrounded()) {
            isCrouched = true;
            isAttackingCurrently = false;
            // Debug.Log("Regular crouch");
        } else {
            if (isWallInDashDirection() && stamina >= 5f) { 
                // Debug.Log("crouching and wall to direction faced: " + isWallInDashDirection());
                // Debug.Log("freezes crouch position on wall");
                isCrouched = true;
                if (!jumpButtonsHeldDown) {
                    // means jump is held down, crouch is held
                    stamina -= wallHangStaminaLossRate * Time.deltaTime;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    // Debug.Log("freezing y pos");
                } else {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                // rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                

            } else {
                // Debug.Log("Regular crouch");
                // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                isCrouched = false;
            }
        }

        // this part happens no matter what
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // returns true if there is no wall in thaat direction
    public bool IsWall(Vector2 direction) {
        float top = GetComponent<Collider2D>().bounds.max.y;
        float bot = GetComponent<Collider2D>().bounds.min.y;
        Vector3 posTop = new Vector3(transform.position.x, top, transform.position.z);
        Vector3 posBot = new Vector3(transform.position.x, bot, transform.position.z);
        RaycastHit2D topHit = Physics2D.Raycast(posTop, direction, sideMoveBoundary, LayerMask.GetMask("Ground"));
        RaycastHit2D botHit = Physics2D.Raycast(posBot, direction, sideMoveBoundary, LayerMask.GetMask("Ground"));

        return topHit.collider == null && botHit.collider == null;
    }

    private void MoveLeftRight(Vector2 direction) {

        if (isAttackingCurrently) {
            return;
        }
        float horizontalInput = 0f;
        Vector2 newVelocity = new Vector2(rb.velocity.x, rb.velocity.y);
        rb.velocity = newVelocity;
        // rb.gravityScale = originalGravityScale;
        // rb.constraints =  RigidbodyConstraints2D.FreezeRotation;

        // isWall(direction)

        if (IsWall(direction) && !isCrouched) {
            // Check if the Y position is locked by comparing constraints
        
            // gfx.Run();
            if (direction == -1 * Vector2.right) {
                horizontalInput = -1f;
            } else {
                horizontalInput = 1f;
            }
        }

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction.x, transform.localScale.y, transform.localScale.z); 

        Vector3 movement = new Vector3(horizontalInput, 0, 0);
        transform.Translate(movement.x * speed * Time.deltaTime, 0, 0);

    }

    private bool isWallInDashDirection() {
        if (transform.localScale.x > 0) {
            return !IsWall(Vector2.right);
            // Debug.Log("Crouch wall to the right");
        } else {
            return !IsWall(-1 * Vector2.right);
            // Debug.Log("Crouch wall to the left");
        }
    }



    private void Dash() {               
        if (canDash && stamina >= dashCost) {

            isAttackingCurrently = false;
            isDashing = true;
            canDash = false;
            // stamina -= dashCost * Time.deltaTime;
            stamina -= dashCost;
            stamina = Mathf.Max(0, stamina);
            
            if (IsGrounded() && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) 
            && isWallInDashDirection()) {
                if (transform.localScale.x > 0) {
                    rb.AddForce(Vector2.right * dashForce * -1, ForceMode2D.Impulse);
                } else {
                    rb.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
                }
                Invoke("TurnOffDashingStatus", dashTimer);
                return;
            }

            if (transform.localScale.x > 0) {
                rb.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
            } else {
                rb.AddForce(-1 * Vector2.right * dashForce, ForceMode2D.Impulse);
            }
            Invoke("TurnOffDashingStatus", dashTimer);
        }
    }

    private void TurnOffDashingStatus() {
        isDashing = false;
        canDash = true;
    }

    public bool IsGrounded()
    {
        // Perform a raycast downwards to check if the player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground", "Breakable"));
        // isInWater
        // Draw the ray in Scene view
        Color rayColor = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, rayColor);

        RaycastHit hitWater;
        if (Physics.Raycast(transform.position, transform.forward, out hitWater, groundCheckDistance, LayerMask.GetMask("Water")))
        {
            // Check if the ray hits something
            Debug.Log("Ray hit: " + hit.transform.name);
            isInWater = true;
        } else {
            isInWater = false;
        }

        return hit.collider != null;
    }

    public bool CrouchingCheck() {
        return isCrouched;
    }

    public bool DashingCheck() {
        return isDashing;
    }

    public float ADCheckStatus() {
        return ADCheck;
    }

    public bool isBeingDamagedCheck() {
        return isBeingDamaged;
    }
    
    public bool WallHangCheck() {
        return (!IsGrounded() && isWallInDashDirection() &&
         stamina >= 5f && !jumpButtonsHeldDown);
    }

    public bool isDeadCheck() {
        return isDead;
    }
    
    void Jump()
    {
        isAttackingCurrently = false;

        if (IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        } else {
            // Debug.Log("Not Grounded");
            if (isCrouched && !IsGrounded()) {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                Debug.Log("isCrouched: " + isCrouched + "    IsGrounded: " + IsGrounded());
                // rb.AddForce(Vector2.up * jumpForce * jumpForce, ForceMode2D.Impulse);
                stamina -= wallHangJumpCost;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Acceleration);
                // rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);

            }
        }
    }



}
