using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public string currentString = "";
    public Text text;


    public GameObject leftPortrait;
    public RectTransform leftTransformOriginal;
    public Image left;


    public GameObject rightPortrait;
    public RectTransform rightTransformOriginal;
    public Image right;
    public float textSpeed = 0.05f; // Global variable for text speed


    private List<dialogueVisuals> dvList;
    private int dvListIndex = 0;
    private Coroutine myCoroutine;



    private GameManager gm;
    private DialogueInstance currentInstance;

    private void Update() {


        if (dvList != null && dvListIndex >= 0 && dvListIndex < dvList.Count) {
            if (Input.GetMouseButtonDown(0)) {
                if (IsCoroutineRunning()) {
                    // Debug.Log("skipped current text");
                    StopCoroutine(myCoroutine);
                    dialogueVisuals dv = dvList[dvListIndex-1];
                    text.text = dv.textString;
                    myCoroutine = null;
                } else {
                    // Debug.Log("Next display triggered");
                    dialogueVisuals dv = dvList[dvListIndex];
                    DisplayDialogue(dv.textString, dv.isOnRightSide, dv.speakerCharacter, dv.offset, dv.sizeScale);
                }
            }
        }

    }

    bool IsCoroutineRunning()
    {
        // Check if the coroutine reference is not null and if the coroutine is still running
        // return myCoroutine != null && !((myCoroutine as Coroutine).IsCompleted);
        return myCoroutine != null;
    }
    public void NewInstance(DialogueInstance instance) {
        if (currentInstance != instance) {
            gm = FindObjectOfType<GameManager>();
            gm.DialogueStateStart();
            currentInstance = instance;
            dialogueVisuals dv = currentInstance.FirstDialogue();
            dvList = currentInstance.FullList();


            GameObject textParent = text.gameObject.transform.parent.gameObject;
            if (textParent != null && !textParent.activeSelf) {
                textParent.SetActive(true);
            }



            DisplayDialogue(dv.textString, dv.isOnRightSide, dv.speakerCharacter, dv.offset, dv.sizeScale);
        }
    }

    private void TransformEdit(Vector2 offset, float scale) {
            Debug.Log("EDIT CALLED");
            float absScale = Mathf.Abs(scale);
            float abovezero = 1;
            if (scale < 0) {
                abovezero = -1;
            }


            leftTransformOriginal.localPosition = new Vector3(offset.x, offset.y, 1.0f);
            leftTransformOriginal.localScale = new Vector3(scale, Mathf.Abs(scale), Mathf.Abs(scale));

            rightTransformOriginal.localPosition = new Vector3(offset.x, offset.y, 1.0f);
            rightTransformOriginal.localScale = new Vector3(scale, Mathf.Abs(scale), Mathf.Abs(scale));
    }

    // Method to display dialogue with an option to display a sprite on the right or left side
    public void DisplayDialogue(string dialogue, bool displayOnRightSide, Sprite character, Vector2 offset, float scale)
    {
        dvListIndex++;


        TransformEdit(offset, scale);

        myCoroutine = StartCoroutine(TypeDialogue(dialogue)); // Start coroutine to type out dialogue

        if (displayOnRightSide)
        {
            // Activate the right portrait
            if (rightPortrait != null && right != null)
            {
                rightPortrait.SetActive(true);
                right.sprite = character;


            }
            // Deactivate the left portrait
            if (leftPortrait != null)
            {
                leftPortrait.SetActive(false);
            }
        }
        else
        {
            // Activate the left portrait
            if (leftPortrait != null && left != null)
            {
                leftPortrait.SetActive(true);
                left.sprite = character;
            }
            // Deactivate the right portrait
            if (rightPortrait != null)
            {
                rightPortrait.SetActive(false);
            }
        }
    }


    public void EndDialogue() {
        GameObject textParent = text.gameObject.transform.parent.gameObject;
        if (textParent != null && textParent.activeSelf) {
            textParent.SetActive(false);
            dvList = null;
            currentInstance = null;
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

        if (text.text == dialogue) {
            myCoroutine = null;
        }
    }

    public enum Trigger
    {
        OnSpawn,
        OnCollider
    }
}
