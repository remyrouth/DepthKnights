using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverActivator : MonoBehaviour
{
    public GameObject targetObject; // The object you want to rotate
    public Vector3 activeRotation = new Vector3(90f, 0f, 0f);
    public float rotationSpeed = 30f; // Adjust the speed as needed

    private bool isSwitchedOn = false;

    public float firstImpactVolume;
    public AudioClip firstImpact;
    public float secondImpactVolume;
    public AudioClip secondImpact;
    private SoundManager sm;
    

    public GameObject bridgeObject;
    private float originalX;
    public GameObject emptyBridgeMove;
    public float bridgeMoveDistance = 4f;
    public float bridgeSpeed = 1f;

    private void Start() {
        // ToggleSwitch();
        sm = FindObjectOfType<SoundManager>();
        // originalPos = bridgeObject.transform.position;
        originalX = bridgeObject.transform.position.x;
    }

    private bool IsBetweenPositions(float position)
    {

        bool isBetween = position < originalX && position > emptyBridgeMove.transform.position.x;

        if (isBetween)
        {
            // Debug.Log($"Position {position} is between originalX ({originalX}) and emptyBridgeMove.x ({emptyBridgeMove.transform.position.x})");
        }

        return isBetween;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     ToggleSwitch();
        // }

        float newXPos = bridgeObject.transform.position.x;

        if (isSwitchedOn)
        {
            RotateTargetObject(activeRotation);

            // MOVING LEFT
            newXPos -= bridgeSpeed * Time.deltaTime;

            if (IsBetweenPositions(newXPos))
            {
                // Debug.Log("Active:      newPos: " + newXPos + " <   emptyBridgeMove: " + emptyBridgeMove.transform.position.x);
                bridgeObject.transform.position = new Vector3(newXPos, bridgeObject.transform.position.y, bridgeObject.transform.position.z);
            }
        }
        else
        {
            // MOVING RIGHT
            RotateTargetObject(Vector3.zero);
            newXPos += bridgeSpeed * Time.deltaTime;

            if (IsBetweenPositions(newXPos))
            {
                // Debug.Log("Disabled:      newPos: " + newXPos + " <   originalX: " + originalX);
                bridgeObject.transform.position = new Vector3(newXPos, bridgeObject.transform.position.y, bridgeObject.transform.position.z);
            }
        }
    }
    public void ToggleSwitch()
    {
        isSwitchedOn = !isSwitchedOn;
        List<AudioClip> temp = new List<AudioClip>();
        temp.Add(firstImpact);
        sm.EnvironmentalSound(temp, firstImpactVolume);
    }

    void RotateTargetObject(Vector3 targetRotation)
    {
        if (targetObject != null)
        {
            // Calculate the rotation step based on rotationSpeed
            float step = rotationSpeed * Time.deltaTime;
            targetObject.transform.rotation = Quaternion.RotateTowards(targetObject.transform.rotation, Quaternion.Euler(targetRotation), step);
        }
    }
}
