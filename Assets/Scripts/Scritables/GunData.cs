using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Game/Gun")]
public class GunData : ScriptableObject
{
    [Header("Gun Attributes")]
    public Sprite gunSprite;
    public int maxBulletCount = 10;
    public int currentBulletCount;
    public float shootSpeed = 0.5f;
    public float offset = 1.0f;

    public GameObject bulletPrefab;




    public float volume = 0.4f;
    public float distanceToHear = 0.5f;
    public AudioClip shootSound;
    public bool loopable = false;
        //public AudioSource newConstantAudio(float volume,
        // float distanceToHear, AudioClip clip, bool loopable, Vector3 soundPosition)

    private void OnEnable()
    {
        // Initialize currentBulletCount to maxBulletCount when the object is created
        currentBulletCount = maxBulletCount;
    }
}
