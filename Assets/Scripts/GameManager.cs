using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Define different states using enums
    public enum ControlState
    {
        Play,
        Menu,
        Dialogue,
        shopMenu,
        Dead
    }
    private ControlState GameState;


    public GatBatController gbc;
    public PlayerController pc;
    private DialogueController dc;
    // this script shoudl activate teh attacks for
    // both the character and the gat bat
    // the other scripts should not be calling this one. 
    public float playerReachRadius = 2f;
    public GameObject cursorObject;
    public GameObject Menu;
    public GameObject shopMenu;
    private SpriteRenderer cursorSprite;
    public Color playerTurnColor;
    public Color batTurnColor;
    public Vector3 offset;




    private SpriteRenderer playerSprite;
    private List<SpriteRenderer> waterList;
    private bool playerInWater = false;


    public void StartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    void Start()
    {
        // cursorObject = transform.Find("Cursor").gameObject;
        cursorSprite = cursorObject.GetComponent<SpriteRenderer>();
        SetUpWaterRegions();
        dc = FindObjectOfType<DialogueController>();
    }

    public void CheckAllInteractables()
    {
        // Get all objects with the tag "Interactable"
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

        // Player position
        Vector3 playerPosition = pc.gameObject.transform.position;

        // List to hold interactables within 2 units of the player
        List<GameObject> nearbyInteractables = new List<GameObject>();

        // Filter interactables to find those within 2 units of the player
        foreach (GameObject interactable in interactables)
        {
            float distance = Vector3.Distance(interactable.transform.position, playerPosition);
            // Debug.Log(distance);
            if (distance <= 2f)
            {
                nearbyInteractables.Add(interactable);
            }
        }

        Debug.Log("Found total interactables: " + nearbyInteractables.Count );
        GameObject closest = GetClosetInteractableToPlayer(nearbyInteractables, playerPosition);


        GameState = ControlState.shopMenu;


    }

    private GameObject GetClosetInteractableToPlayer(List<GameObject> nearbyInteractables, Vector3 playerPos) {
        // Find the closest interactable
        GameObject closestInteractable = nearbyInteractables[0];
        float closestDistance = Vector3.Distance(closestInteractable.transform.position, playerPos);

        foreach (GameObject interactable in nearbyInteractables)
        {
            float distance = Vector3.Distance(interactable.transform.position, playerPos);
            if (distance < closestDistance)
            {
                closestInteractable = interactable;
                closestDistance = distance;
            }
        }

        return closestInteractable;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuControl();
        }


        Vector3 currentMousePosition = GetMousePosition();
        bool isPlayerTurn = IsPlayerTurn(currentMousePosition);

        switch (GameState)
        {
            case ControlState.Play:
                pc.ChangePlayStatus(true);
                if (Menu != null)
                {
                    Menu.SetActive(false);
                    shopMenu.SetActive(false);
                }
                PlayerClickControls(isPlayerTurn, currentMousePosition);
                MoveCursor(currentMousePosition, isPlayerTurn);
                break;

            case ControlState.Menu:

                pc.ChangePlayStatus(false);
                if (Menu != null)
                {
                    Menu.SetActive(true);
                    shopMenu.SetActive(false);
                }
                // Menu.SetActive(true);
                MoveCursor(currentMousePosition, isPlayerTurn);
                break;

            case ControlState.shopMenu:

                pc.ChangePlayStatus(false);
                if (shopMenu != null)
                {
                    shopMenu.SetActive(true);
                    Menu.SetActive(false);
                }
                MoveCursor(currentMousePosition, isPlayerTurn);
                break;

            case ControlState.Dialogue:

                pc.ChangePlayStatus(false);
                if (Menu != null)
                {
                    Menu.SetActive(false);
                    shopMenu.SetActive(false);
                }
                NullifyCrossHair();
                break;

            case ControlState.Dead:

                if (Menu != null)
                {
                    Menu.SetActive(false);
                    shopMenu.SetActive(false);
                }
                pc.ChangePlayStatus(false);

                break;
        }

    }

    public void DialogueStateStart()
    {
        GameState = ControlState.Dialogue;
    }

    private void NullifyCrossHair()
    {
        cursorSprite.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void MenuControl()
    {
        switch (GameState)
        {
            case ControlState.Play:
                GameState = ControlState.Menu;
                break;

            default:
                GameState = ControlState.Play;
                dc.EndDialogue();
                break;
        }
    }

    private void PlayerClickControls(bool isPlayerTurn, Vector3 currentMousePosition)
    {


        if (Input.GetMouseButtonDown(0))
        {
            if (isPlayerTurn)
            {
                // Debug.Log("attack debug activated");
                pc.StartEdenAttackType1();
            }
            else
            {
                // gbc.SetTargetPosition(currentMousePosition);
                if (gbc != null)
                {
                    gbc.ShootBullet();
                }
                // gbc.ShootBullet();
            }
        }

    }

    void MoveCursor(Vector3 newPos, bool colorDetermination)
    {
        cursorObject.transform.position = new Vector3(newPos.x + offset.x, newPos.y + offset.y, 0f);

        if (colorDetermination)
        {
            cursorSprite.color = playerTurnColor;
        }
        else
        {
            cursorSprite.color = batTurnColor;
        }
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }

    public bool IsPlayerTurn(Vector3 positionClicked)
    {     // its taking vector3, it should be taking vector2
        Vector3 mousPos = GetMousePosition();
        mousPos.z = 0f;
        float distance = Vector3.Distance(pc.gameObject.transform.position, mousPos);
        // Debug.Log("Distance from player: " + distance);
        return (distance < playerReachRadius);
    }

    public void InitializeBat(GatBatController newBat)
    {
        gbc = newBat;
    }

    public void InitializeEden(PlayerController newEden)
    {
        pc = newEden;
    }


    private void SetUpWaterRegions()
    {
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

    public bool PlayerInWaterCheck()
    {
        float posX = playerSprite.bounds.center.x;
        float posY = playerSprite.bounds.min.y;

        // Log the names of the objects to the console
        foreach (SpriteRenderer waterSprite in waterList)
        {
            // Debug.Log("Object on Water layer: " + waterSprite.name);
            if (posX < waterSprite.bounds.max.x && posX > waterSprite.bounds.min.x &&
                posY < waterSprite.bounds.max.y && posY > waterSprite.bounds.min.y)
            {
                return true;
            }
        }
        // return playerInWater;
        return false;
    }


}
