using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabController : MonoBehaviour
{
    // 1 = run
    // 2 = death
    // 3 = attack3
    // 4 = attack2
    // 5 = attack1
    // 6 = ability

    public BossState crabState = BossState.Idle;

    [Range(0, 100)]
    public int closeRangePercentageChance = 50; // Percentage chance of getting true
    public float moveSpeed = 4f;
    public float closeRangeTriggerRange = 1.2f;

    public List<int> ShortRangeAnimInts = new List<int>();
    public List<int> LongRangeAnimInts = new List<int>();

    public float InitalAgroDelay = 2f;



    private PlayerController pc;
    private Animator ani;

    public enum BossState {
        Run,
        AttackCloseRange,
        AttackFarRange,
        Hurt,
        Death,
        Idle
    }


    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponentInChildren<Animator>();
        pc = FindObjectOfType<PlayerController>();

        ResetAfterAttack(InitalAgroDelay);
    }

    private void Update() {
        StateMachine(crabState);
    }

    private bool canChooseNewAttack = true;

    private void StateMachine(BossState newState) {
        switch (newState) {
            case BossState.Run:
                FacePlayer();  
                MoveForward();
                SetAnimationState(1);

                if (CanActivateCloseRange()) {
                    crabState = BossState.AttackCloseRange;
                }
                break;

            case BossState.AttackCloseRange:
                // we can either choose to start running towards the player and then  choose a meelee attack
                // or we can just attack using the long range option immediatly

                if (canChooseNewAttack) {
                    // 3, 4, 5
                    int aniInt = SelectRandomInt(ShortRangeAnimInts);
                    SetAnimationState(aniInt);

                    canChooseNewAttack = false;
                }

                    
                break;


            case BossState.AttackFarRange:
                if (canChooseNewAttack) {
                    // 6
                    int aniInt = SelectRandomInt(LongRangeAnimInts);
                    SetAnimationState(aniInt);

                    canChooseNewAttack = false;
                }
                    
                break;

            case BossState.Hurt:
                    
                break;

            case BossState.Idle:
                    FacePlayer();
                    SetAnimationState(0);
                    canChooseNewAttack = true;
                break;                
        }
    }

    // Triggered by animation controller frames
    public void ResetAfterAttack(float idleDuration) {
        // Debug.Log("ResetAfterAttack actived");
        // idleDuration is how long we'll be in idle state before we attack
        crabState = BossState.Idle;
        SetAnimationState(0);
        Invoke("LeaveIdleState", idleDuration);
    }

    private void LeaveIdleState() {
        SetAnimationState(0);
        bool isCloseRange = AttackTypeChooser();

        if (isCloseRange) {
            crabState = BossState.Run;
            // Debug.Log("Now Running");
        } else {
            crabState = BossState.AttackFarRange;
            // Debug.Log("Now Attacking from long range");
        }
    }

    private bool CanActivateCloseRange() {
        float distance = Vector3.Distance(pc.gameObject.transform.position, transform.position);

        if (distance <= closeRangeTriggerRange) {
            return true;
        } else {
            return false;
        }
    }

    private void MoveForward() {
        Vector3 direction = Vector2.left;

        if (transform.localScale.x > 0) {
            // means we are facing to the right
            direction = Vector2.right; 
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime);

    }

    
    private void FacePlayer() {
        // scale of x = -1 means the crab faces left
        // scale of x = 1 means the crab faces right
        float playerX = pc.gameObject.transform.position.x;
        float crabX = transform.position.x;

        if (playerX > crabX) {
            float newCrabX = Mathf.Abs(transform.localScale.x) * 1;
            transform.localScale = new Vector3(newCrabX, transform.localScale.y, transform.localScale.z);
        } else {
            float newCrabX = Mathf.Abs(transform.localScale.x) * -1;
            transform.localScale = new Vector3(newCrabX, transform.localScale.y, transform.localScale.z);
        }
    }


    private void StartAttack() {

    }

    private bool AttackTypeChooser()
    {
        // if true, then we actate a close range
        // else long range

        int randomNumber = Random.Range(0, 100); // Generate a random number between 0 and 100

        // Return true if the random number is less than the true percentage
        // if random number is less than the close range percentage chance, then we choose close range
        bool isCloseRange =  randomNumber < closeRangePercentageChance;
        return isCloseRange;
    }
    
    private void SetAnimationState(int state)
    {
        if (ani.GetInteger("AnimState") != state)
        {
            ani.SetInteger("AnimState", state);
        }
    }

    public int GetCurrentAnimationState()
    {
        // Return the current value of the "AnimState" parameter
        return ani.GetInteger("AnimState");
    }

    public int SelectRandomInt(List<int> integerList)
    {
        // Check if the list is empty
        if (integerList.Count == 0)
        {
            Debug.LogError("The integer list is empty!");
            return 0; // Return a default value or handle the error appropriately
        }

        // Generate a random index within the range of the list
        int randomIndex = Random.Range(0, integerList.Count);

        // Return the integer at the randomly selected index
        return integerList[randomIndex];
    }
}
