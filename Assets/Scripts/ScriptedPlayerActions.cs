using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedPlayerActions : MonoBehaviour
{
    PlayerController pc;
    [SerializeField]
    public enum PlayerActionTrigger
    {
        WalkRight,
        WalkLeft,
        Idle,
        EndController
    }

    [System.Serializable]
    public class ActionTypes
    {
        public PlayerActionTrigger action;
        public float actionDuration;
    }

    public bool willTransition = false;
    public float transitionRate = 0f;
    private CameraToPlayer ctp;

    
    public List<ActionTypes> actions = new List<ActionTypes>();

    // Start is called before the first frame update
    void Start()
    {
        pc = FindObjectOfType<PlayerController>();


    }

    private bool alreadyTriggered = false;
    // This method is called when another collider enters the trigger zone

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collider for solider collider triggered ");
        // Debug.Log("Collider for scripted actions triggered");

        if (collision.gameObject.CompareTag("Player")) {
            if (alreadyTriggered)
            {
                return;
            }

            pc.ChangePlayStatus(false);
            if (willTransition) {
                ctp = Camera.main.GetComponent<CameraToPlayer>();
                ctp.StartTransition(transitionRate);
            }

            float currentTime = 0f;
            for (int i = 0; i < actions.Count; i++)
            {
                HandleIndividual(actions[i], currentTime);
                currentTime += actions[i].actionDuration;
            }
        }


        // Deactivate all colliders on the current GameObject
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void HandleIndividual(ActionTypes action, float currentTime)
    {
        // Debug.Log("action type: " + action.action);
        if (action.action == PlayerActionTrigger.WalkRight)
        {
            // Debug.Log("");
            Invoke("StartWalkRight", currentTime);
            Invoke("EndWalkRight", currentTime + action.actionDuration);
        }
        else if (action.action == PlayerActionTrigger.EndController)
        {
            Invoke("EndScript", currentTime + action.actionDuration);
        }
    }

    private void StartWalkRight()
    {
        Debug.Log("Walked right from controller");
        pc.AutoWalkRight(true);
    }

    private void EndWalkRight()
    {
        // Debug.Log("Walked right end");
        pc.AutoWalkRight(false);
    }

    private void EndScript() {
        pc.ChangePlayStatus(true);
        Destroy(gameObject);
    }
}
