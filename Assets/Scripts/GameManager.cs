using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Define different states using enums
    public enum ControlState
    {
        Play,
        Menu,
        Dialogue,
        ShopMenu,
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
    public GameObject ShopMenu;
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

    public void checkdistancetoallinteractables()
    {
        // collect all objects with ineractale tag
        // choose closest one that is also within distance of 3f

        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Interactable");
        GameObject closestObject = null;
        Vector3 playerPosition = pc.gameObject.transform.position;
        float closestDistance = Mathf.Infinity;
        float searchRange = 3f;
        

        foreach (GameObject obj in objectsWithTag)
        {
            float distanceToPlayer = Vector3.Distance(obj.transform.position, playerPosition);
            if (distanceToPlayer <= searchRange && distanceToPlayer < closestDistance)
            {
                closestObject = obj;
                closestDistance = distanceToPlayer;
            }
        }

        if (closestObject != null)
        {
            Debug.Log("Closest Object Found: " + closestObject.name);
            //if(closestObject.name == "Shop"){
            //    GameState = ControlState.ShopMenu;
            //}
        }
        else
        {
            Debug.Log("No objects within range found.");
        }

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
                }
                PlayerClickControls(isPlayerTurn, currentMousePosition);
                MoveCursor(currentMousePosition, isPlayerTurn);
                break;

            case ControlState.Menu:

                pc.ChangePlayStatus(false);
                if (Menu != null)
                {
                    Menu.SetActive(true);
                }
                // Menu.SetActive(true);
                MoveCursor(currentMousePosition, isPlayerTurn);
                break;

            case ControlState.ShopMenu:

                pc.ChangePlayStatus(false);
                if (ShopMenu != null)
                {
                    ShopMenu.SetActive(true);
                }
                MoveCursor(currentMousePosition, isPlayerTurn);
                break;

            case ControlState.Dialogue:

                pc.ChangePlayStatus(false);
                if (Menu != null)
                {
                    Menu.SetActive(false);
                }
                NullifyCrossHair();
                break;

            case ControlState.Dead:

                if (Menu != null)
                {
                    Menu.SetActive(false);
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
