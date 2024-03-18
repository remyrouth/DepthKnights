using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public string currentString = "";
    public Text text;
    public GameObject leftPortrait;
    private SpriteRenderer left;
    public GameObject rightPortrait;
    private SpriteRenderer right;
    public float textSpeed = 0.05f; // Global variable for text speed


    private DialogueInstance currentInstance;

    private void Start() {
        right = rightPortrait.GetComponent<SpriteRenderer>();
        left = leftPortrait.GetComponent<SpriteRenderer>();

        // DisplayDialogue(currentString, true);
    }

    public void NewInstance(DialogueInstance instance) {
        if (currentInstance != instance) {
            currentInstance = instance;
        }
    }

    // Method to display dialogue with an option to display a sprite on the right or left side
    public void DisplayDialogue(string dialogue, bool displayOnRightSide, Sprite character)
    {
        StartCoroutine(TypeDialogue(dialogue)); // Start coroutine to type out dialogue
        
        if (displayOnRightSide)
        {
            // Activate the right portrait
            if (rightPortrait != null)
                rightPortrait.SetActive(true);
                right.sprite = character;
            // Deactivate the left portrait
            if (leftPortrait != null)
                leftPortrait.SetActive(false);
        }
        else
        {
            // Activate the left portrait
            if (leftPortrait != null)
                leftPortrait.SetActive(true);
                left.sprite = character;
            // Deactivate the right portrait
            if (rightPortrait != null)
                rightPortrait.SetActive(false);
        }
    }

    // Coroutine to type out dialogue
    IEnumerator TypeDialogue(string dialogue)
    {
        text.text = ""; // Clear the text initially

        foreach (char letter in dialogue)
        {
            text.text += letter; // Append each character to the text
            yield return new WaitForSeconds(textSpeed); // Wait for textSpeed seconds
        }
    }

    public enum Trigger
    {
        OnSpawn,
        OnCollider
    }
}
