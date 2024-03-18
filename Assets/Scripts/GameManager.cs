using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GatBatController gbc;
    public PlayerController pc;


    // this script shoudl activate teh attacks for
    // both the character and the gat bat
    // the other scripts should not be calling this one. 
    public float playerReachRadius = 2f;
    public GameObject cursorObject;
    public GameObject Menu;
    private SpriteRenderer cursorSprite;
    public Color playerTurnColor;
    public Color batTurnColor;
    public Vector3 offset;




    private SpriteRenderer playerSprite;
    private List<SpriteRenderer> waterList;
    private bool playerInWater = false;


    void Start()
    {
        // cursorObject = transform.Find("Cursor").gameObject;
        cursorSprite = cursorObject.GetComponent<SpriteRenderer>();
        SetUpWaterRegions();
    }

    private void Update()
    {
        Vector3 currentMousePosition = GetMousePosition();
        bool isPlayerTurn = IsPlayerTurn(currentMousePosition);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuControl();
        }

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

        if (Input.GetMouseButtonDown(1))
        {
            if (isPlayerTurn)
            {
                // Debug.Log("attack debug activated");
                // pc.StartEdenAttackType2();
            }
            else
            {
                if (gbc != null)
                {
                    gbc.SetTargetPosition(currentMousePosition);
                }
                // gbc.SetTargetPosition(currentMousePosition);s
                // gbc.ShootBullet();
            }
        }

        MoveCursor(currentMousePosition, isPlayerTurn);

        // DrawCircle();
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

    public void MenuControl()
    {
        if (!Menu.activeInHierarchy)
        {
            Menu.SetActive(true);
            pc.ChangePlayStatus(false);
        }
        else
        {
            Menu.SetActive(false);
            pc.ChangePlayStatus(true);
        }
    }

}
