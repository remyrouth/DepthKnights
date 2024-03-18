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


    void Start()
    {
        // cursorObject = transform.Find("Cursor").gameObject;
        cursorSprite = cursorObject.GetComponent<SpriteRenderer>();
    }

    private void Update() {
        Vector3 currentMousePosition = GetMousePosition();
        bool isPlayerTurn = IsPlayerTurn(currentMousePosition);
        if (Input.GetKeyDown(KeyCode.Escape))
        {   
            if(!Menu.activeInHierarchy){
                Menu.SetActive(true);
                pc.ChangePlayStatus(false);
            }else{
                Menu.SetActive(false);
                pc.ChangePlayStatus(true);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isPlayerTurn) {
                // Debug.Log("attack debug activated");
                pc.StartEdenAttackType1();
            } else {
                // gbc.SetTargetPosition(currentMousePosition);
                if (gbc != null) {
                    gbc.ShootBullet();
                }
                // gbc.ShootBullet();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isPlayerTurn) {
                // Debug.Log("attack debug activated");
                // pc.StartEdenAttackType2();
            } else {
                if (gbc != null) {
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

        if (colorDetermination) {
            cursorSprite.color = playerTurnColor;
        } else {
            cursorSprite.color = batTurnColor;
        }
    }

    public Vector3 GetMousePosition() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }

    public bool IsPlayerTurn(Vector3 positionClicked) {     // its taking vector3, it should be taking vector2
        Vector3 mousPos = GetMousePosition();
        mousPos.z = 0f;
        float distance = Vector3.Distance(pc.gameObject.transform.position, mousPos);
        // Debug.Log("Distance from player: " + distance);
        return (distance < playerReachRadius);
    }

    public void InitializeBat(GatBatController newBat) {
        gbc = newBat;
    }

    public void InitializeEden(PlayerController newEden) {
        pc = newEden;
    }



}
