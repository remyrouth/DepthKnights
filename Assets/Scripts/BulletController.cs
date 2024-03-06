using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 1f;
    public float speed = 5f;

    void Update()
    {
        // Move the bullet in its forward direction
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
