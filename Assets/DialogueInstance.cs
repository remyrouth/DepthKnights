using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInstance : MonoBehaviour
{
    private DialogueController dc;
    public Trigger startOption;
    public List<dialogueVisuals> dialogueSequence = new List<dialogueVisuals>();

    // Start is called before the first frame update
    void Start()
    {
        dc = FindObjectOfType<DialogueController>();
        if (startOption == Trigger.OnSpawn) {
            // Debug.Log("Instanced method triggered");
            dc.NewInstance(this);
        }
    }

    public dialogueVisuals FirstDialogue() {
        return dialogueSequence[0];
    } 

    public List<dialogueVisuals> FullList() {
        return dialogueSequence;
    }

    public enum Trigger {
        OnSpawn,
        OnCollider
    }
}

[System.Serializable]
public class dialogueVisuals {
    public string textString = "";
    public Sprite speakerCharacter;
    public bool isOnRightSide = true;
    public Vector2 offset;
    public float sizeScale = 1f;
}
