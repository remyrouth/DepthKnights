using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    private SoundManager sm;
    public float volume = 0.8f;
    public float maxDistance = 3f;
    public string SceneString = "MainMenu";
    public AudioClip clip;

    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        sm = FindObjectOfType<SoundManager>();
        sm.newConstantAudio(volume, maxDistance, clip, true, transform.position);

        gm = FindObjectOfType<GameManager>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("INSTANCE COLLIDER KNOCKED");
        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("INSTANCE COLLIDER BY PLAYER");
            // Debug.Log("Collision Enter with player: " + collision.gameObject.name);
            // Additional actions specific to collision with the player
            gm.StartScene(SceneString);
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null) {
                Destroy(collider);
            }
            gameObject.SetActive(false);
        }
    }

}
