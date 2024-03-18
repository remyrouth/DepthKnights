using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    public bool slowTime = false;
    private PlayerController pc;

    private Animator animator;


    private bool touchingGround = false;
    private bool touchingLeftWall = false;
    private bool touchingRightWall = false;
    private bool isCrouched = false;
    private bool isDashing = false;
    private float horizontalRecord = 0f;
    private bool isWallHanging = false;
    private bool isAttackingWithType1 = false;
    private bool isBeinghurt = false;
    private Rigidbody2D rb;
    // Previous position of the GameObject
    private Vector2 previousPosition;


    // Sound
    private SoundManager sm;
    private GameManager gm;
    public float footStepVolume = 0.8f;
    public List<AudioClip> footStepClips = new List<AudioClip>();
    public float waterStepVolume = 0.8f;
    public List<AudioClip> waterStepClips = new List<AudioClip>();

    public float swordVolume = 0.8f;
    public List<AudioClip> swordSwingClips = new List<AudioClip>();
    public List<AudioClip> JumpClips = new List<AudioClip>();
    public float dashVolume = 0.8f;
    public List<AudioClip> dashClips = new List<AudioClip>();
    public List<AudioClip> wallHangClips = new List<AudioClip>();

    
    // 0 = idle
    // 1 = run
    // 2 = jump up
    // 3 = jump peak
    // 4 = jump fall

    // 5 = crouched
    // 6 = dashing
    // 7 = wall hang
    // 8 = attack

    // 9 = being damaged/hurt
    // 10 = dead
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        previousPosition = rb.position;

        pc = transform.parent.parent.GetComponent<PlayerController>();
        animator = GetComponent<Animator>(); 
        sm = FindObjectOfType<SoundManager>();
        gm = FindObjectOfType<GameManager>();

    }

    public void StepSoundSoundTrigger() {
        if (gm.PlayerInWaterCheck()) {
            sm.PlayerSound(waterStepClips, waterStepVolume);
        } else {
            sm.PlayerSound(footStepClips, footStepVolume);
        }
        // sm.PlayerSound(footStepClips, footstepVolume);
    }

    public void SwordSwingSoundTrigger() {
        sm.PlayerSound(swordSwingClips, swordVolume);
    }

    public void DashSoundTrigger() {
        sm.PlayerSound(dashClips, dashVolume);
    }

    void Update()
    {
        if (slowTime) {
            Time.timeScale = 0.5f;
        } else {
            Time.timeScale = 1f;
        }
        touchingRightWall = pc.IsWall(Vector2.right);
        touchingLeftWall = pc.IsWall(Vector2.left);
        touchingGround = pc.IsGrounded();
        isCrouched = pc.CrouchingCheck();
        isDashing = pc.DashingCheck();
        horizontalRecord = pc.ADCheckStatus();
        isWallHanging = pc.WallHangCheck();
        isAttackingWithType1 = pc.Attack1Check();
        isBeinghurt = pc.isBeingDamagedCheck();
        

        HandleGFX();

    }

    private void EndAttack1() {
        pc.EndEdenAttackType1();
    }

    private void EndHurtStatus() {
        pc.EndBeingDamagedStatus();
    }

    private void TriggerFrameAttackDamage() {
        SwordSwingSoundTrigger();
        pc.DealDamageOnTriggerAttackFrame();
    }

    private void JumpAnimationChooser()
    {

        float displacement = rb.position.y - previousPosition.y;

        if (displacement > 0) {         // rising
            // Debug.Log("Moving Up");
            SetAnimationState(2);
        } else {                       // falling
            // Debug.Log("Moving Down");
            SetAnimationState(4);
        }


    }


    private void HandleGFX() {
        if (isBeinghurt) {
            SetAnimationState(9);
            return;
        }

        // Dashing -> priority over everything but wallhang
        if (isAttackingWithType1) {
            SetAnimationState(8);
            return;
        }

        if (!touchingGround) {
            // DashAnimation
            if (isDashing) {
                SetAnimationState(6); // dash
            // } else if (touchingGround && (touchingLeftWall || touchingRightWall)) {
            } else if (!isWallHanging) {
                // SetAnimationState(7);  // hall hang
                JumpAnimationChooser();
                
            } else {
                // JumpAnimationChooser();
                SetAnimationState(7);  // hall hang
            }
        } else {

            if(isCrouched) {
                SetAnimationState(5); // run
                return;
            }

            float displacement = rb.position.x - previousPosition.x;
            if (displacement == 0) {
                SetAnimationState(0); // idle
            } else {
                if (isDashing) {
                    SetAnimationState(6); // dash
                } else {
                    SetAnimationState(1); // run
                }

            }
            // previousPosition = rb.position;
        }
        previousPosition = rb.position;
        // Debug.Log("touchingGround Status: " + touchingGround);
    }

    private void SetAnimationState(int state)
    {
        if (animator.GetInteger("AnimState") != state)
        {
            animator.SetInteger("AnimState", state);
        }
    }


}
