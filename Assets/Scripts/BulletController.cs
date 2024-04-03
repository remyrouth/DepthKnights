using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 1f;
    public float speed = 5f;

    public bool isPlayerShot = true;

    void Update()
    {
        // Move the bullet in its forward direction
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy the game object after 2 seconds
        // Destroy(gameObject, 2f);
        // Check if the collision is with another object
        EnemyController ec = other.gameObject.GetComponent<EnemyController>();
        if (ec != null) {
            Debug.Log("Enemy hit");
            ec.DecreaseHealth();
            // DealDamageOnTriggerAttackFrame();
            // return;
        }

        PlayerController pc = other.gameObject.GetComponent<PlayerController>();
        // if (!isPlayerShot && pc == null)  {
        //     Destroy(gameObject);
        // }
        if (pc == null)  {
            Destroy(gameObject);
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the game object after 2 seconds
        // Destroy(gameObject, 2f);
        // Check if the collision is with another object
        EnemyController ec = collision.gameObject.GetComponent<EnemyController>();
        if (ec != null) {
            Debug.Log("Enemy hit");
            ec.DecreaseHealth();
            // DealDamageOnTriggerAttackFrame();
            return;
        }

        Destroy(gameObject);
    }
}
