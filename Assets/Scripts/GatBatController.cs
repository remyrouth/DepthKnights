using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatBatController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 targetPosition;

    private GameObject batGFX;
    private GameObject gunHolster;
    public GunData gunChosen;
    private GameManager gm;

    private PlayerController pc;
    private GameObject PlayerBatHoldPosition;

    public float maxDistanceAllowed = 5f;

    private void Start() {
        targetPosition = new Vector2(transform.position.x, transform.position.y);
        batGFX = transform.Find("BatHolder").Find("GatBat").gameObject;
        gunHolster = transform.Find("GunHolster").gameObject;
        ChangeGunSprite(gunChosen.gunSprite);
        gm = FindObjectOfType<GameManager>();
        gm.InitializeBat(this);

        pc = FindObjectOfType<PlayerController>();
        PlayerBatHoldPosition = pc.BatPosFinder();
    }

    private void Update()
    {
        LookAtMouse();

        DistanceCheck();

        SetTargetPosition(PlayerBatHoldPosition.transform.position);
        MoveTowardsTarget();
    }

    private void DistanceCheck() {
        float distance = Vector3.Distance(transform.position, PlayerBatHoldPosition.transform.position);
        if (distance > maxDistanceAllowed) {
            transform.position = PlayerBatHoldPosition.transform.position;
        }
    }

    public void ShootBullet() {
        // Check if the gunChosen is not null and has a bullet prefab
        if (gunChosen != null && gunChosen.bulletPrefab != null) {
            // Instantiate a new bullet from the gun's bullet prefab
            GameObject bulletObj = Instantiate(gunChosen.bulletPrefab, gunHolster.transform.position, gunHolster.transform.rotation);
            
            // // Access the BulletController script on the instantiated bullet
            // BulletController bulletController = bulletObj.GetComponent<BulletController>();

            // // Set bullet damage and speed based on gunChosen properties
            // bulletController.damage = gunChosen.bulletDamage;
            // bulletController.speed = gunChosen.bulletSpeed;
        }
    }

    void LookAtMouse() {
        // Get the mouse position in world space
        Vector3 mousePosition = gm.GetMousePosition();

        // Calculate the direction to the mouse position
        Vector3 direction = mousePosition - transform.position;
        direction.z = 0f; // Ensure the object stays in the 2D plane

        // Calculate the angle to rotate towards the mouse position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set the rotation to face the mouse position instantly
        gunHolster.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (IsPointToLeft(mousePosition)) {
            gunHolster.transform.localScale = new Vector3(gunHolster.transform.localScale.x,
             Mathf.Abs(gunHolster.transform.localScale.y), gunHolster.transform.localScale.z);
        } else {
            gunHolster.transform.localScale = new Vector3(gunHolster.transform.localScale.x,
             Mathf.Abs(gunHolster.transform.localScale.y) * -1, gunHolster.transform.localScale.z);
        }
    }

    private void ChangeGunSprite(Sprite newGunSprite) {
        gunHolster.transform.Find("GunSprite").gameObject.GetComponent<SpriteRenderer>().sprite = newGunSprite;
    }

    private void SetTargetPosition(Vector3 mousPos)
    {
        targetPosition = new Vector2(mousPos.x, mousPos.y);
        
        if (IsPointToLeft(targetPosition)) {
            batGFX.transform.localScale = new Vector3(Mathf.Abs(batGFX.transform.localScale.x),
             batGFX.transform.localScale.y, batGFX.transform.localScale.z);
        } else {
            batGFX.transform.localScale = new Vector3(Mathf.Abs(batGFX.transform.localScale.x) * -1,
             batGFX.transform.localScale.y, batGFX.transform.localScale.z);
        }
    }

    public bool IsPointToLeft(Vector2 point)
    {
        // Get the player's position
        Vector2 playerPosition = transform.position;

        // Calculate the direction from the player to the point
        Vector2 directionToTarget = point - playerPosition;

        // Get the player's forward direction (assuming player is facing right)
        Vector2 playerForward = transform.right;

        // Calculate the dot product to determine if the point is to the left or right
        float dotProduct = Vector2.Dot(directionToTarget, playerForward);

        // If dot product is greater than 0, the point is to the right, else it's to the left
        return dotProduct > 0f;
    }

    void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check for collision with obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetPosition - (Vector2)transform.position, moveSpeed * Time.deltaTime);
        if (hit.collider != null)
        {
            // If collision occurs, stop moving
            targetPosition = transform.position;
        }
    }
}
