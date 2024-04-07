using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerButtonController : MonoBehaviour
{
    [Header("Button Movement Variables")]
    public GameObject buttonObject;
    private float yUpPos;
    private float yDownPos;
    public float yOffset = 0.5f;
    public float buttonMoveSpeed;

    [Header("Tyrant Object Variables")]
    public GameObject Tyrant;
    public GameObject TyrantEndLerpPos;
    public AnimationCurve curveSpeed;
    public float tyrantBaseSpeed;
    private float totalMoveDistance;

    [Header("Bounds Variables")]
    public Vector2 offset = new Vector2(100, 100); // Offset from the top-left corner of the screen
    public Vector2 size = new Vector2(200, 100);   // Size of the rectangle

    public float lineWidth = 0.1f;
    public Color color = Color.white;

    [Header("Sounds Variables")]
    public AudioClip bellClip;
    public float bellVolume;
    public float distanceToHear;
    public bool Loopable = false;
    private SoundManager sm;


    private void Start() {
        totalMoveDistance = Vector3.Distance(Tyrant.transform.position, TyrantEndLerpPos.transform.position);
        yUpPos = buttonObject.transform.position.y;
        yDownPos = buttonObject.transform.position.y - yOffset;

        sm = FindObjectOfType<SoundManager>();
    }

    // private void OnDrawGizmosSelected()
    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        // Vector3 pos = transform.position;
        Vector3 pos = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z);
        Vector3 halfSize = new Vector3(size.x / 2, size.y / 2, 0);

        // Draw the rectangle
        Gizmos.DrawLine(pos + new Vector3(-halfSize.x, -halfSize.y, 0), pos + new Vector3(halfSize.x, -halfSize.y, 0));
        Gizmos.DrawLine(pos + new Vector3(halfSize.x, -halfSize.y, 0), pos + new Vector3(halfSize.x, halfSize.y, 0));
        Gizmos.DrawLine(pos + new Vector3(halfSize.x, halfSize.y, 0), pos + new Vector3(-halfSize.x, halfSize.y, 0));
        Gizmos.DrawLine(pos + new Vector3(-halfSize.x, halfSize.y, 0), pos + new Vector3(-halfSize.x, -halfSize.y, 0));

        // Draw lines for width
        Gizmos.DrawLine(pos + new Vector3(-halfSize.x, -halfSize.y - lineWidth / 2, 0), pos + new Vector3(halfSize.x, -halfSize.y - lineWidth / 2, 0));
        Gizmos.DrawLine(pos + new Vector3(halfSize.x + lineWidth / 2, -halfSize.y, 0), pos + new Vector3(halfSize.x + lineWidth / 2, halfSize.y, 0));
        Gizmos.DrawLine(pos + new Vector3(halfSize.x, halfSize.y + lineWidth / 2, 0), pos + new Vector3(-halfSize.x, halfSize.y + lineWidth / 2, 0));
        Gizmos.DrawLine(pos + new Vector3(-halfSize.x - lineWidth / 2, halfSize.y, 0), pos + new Vector3(-halfSize.x - lineWidth / 2, -halfSize.y, 0));
    }

    private bool canActivate = true;

    public void ButtonActivate() {
        Debug.Log("Button Activated");

        if (canActivate) {
            StartCoroutine(MoveObjectDownward());  
            StartCoroutine(StartTyrantMove());  

            // sm.newConstantAudio

            canActivate = false;     
        }
    }

    IEnumerator StartTyrantMove()
    {
        while (Vector3.Distance(TyrantEndLerpPos.transform.position, Tyrant.transform.position) > 0.01f)
        {
            // Calculate the direction towards the target
            Vector3 direction = TyrantEndLerpPos.transform.position - Tyrant.transform.position;
            
            // Normalize the direction to get a unit vector
            direction.Normalize();

            float distanceLeft = Vector3.Distance(Tyrant.transform.position, TyrantEndLerpPos.transform.position);
            float percentageTraveled = distanceLeft/totalMoveDistance;
            float curveValue = curveSpeed.Evaluate(percentageTraveled);
            
            // Move the object towards the target
            Tyrant.transform.position += direction * tyrantBaseSpeed * curveValue * Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches exactly to the target position
        Tyrant.transform.position = TyrantEndLerpPos.transform.position;

        // Optional: You can perform any action after reaching the target position here
    }

    IEnumerator MoveObjectDownward()
    {
        // Move the object downward until it reaches the target Y position
        while (buttonObject.transform.position.y < yDownPos)
        {
            // Calculate the new position based on the move speed
            Vector3 newPosition = buttonObject.transform.position + Vector3.down * buttonMoveSpeed * Time.deltaTime;

            // Update the object's position
            buttonObject.transform.position = newPosition;

            // Wait for the end of the frame
            yield return null;
        }

        // // Snap the object to the target Y position to ensure it reaches it exactly
        // transform.position = new Vector3(transform.position.x, targetY, transform.position.z);

        // End of movement
        Debug.Log("Object reached target Y position!");
    }


}
