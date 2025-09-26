using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem; // For the new Input System

public class PlayerController : MonoBehaviour
{
    [Header("Player Speed")]
    public float moveSpeed = 5f;

    [Header("Player Health")]
    public float health = 100f;

    [Header("Punch Range")]
    public float punchRange = 2f;

    [Header("Jab Damage")]
    public float jabDamage = 20f;

    [Header("Hook Damage")]
    public float hookDamage = 30f;

    [Header("Punch Cooldown")]
    public float punchCooldown;

    [Header("Enemy Transform")]
    public Transform enemy;

    [Header("Hook Sound effect")]
    public AudioClip hookSFX;

    [Header("Jab Sound effect")]
    public AudioClip jabSFX;

    [Header("SFX Volume")]
    public float sfxVolume;

    private Animator animator;
    private Rigidbody rb;
    private bool canPunch = true;

    void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    void Update()
    {
        if (enemy == null) return;

        float distance = Vector3.Distance(transform.position, enemy.position);
        // Move towards enemy if it isn't in range to punch (with 0.5 extra to make it look smoother)
        Vector3 direction = (enemy.position - transform.position).normalized;
        direction.y = 0f; // so that it doesn't "fly" with the y-axis
        if (distance > punchRange - 0.5f)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        // Look towards enemy
        transform.rotation = Quaternion.LookRotation(direction);

        // Jab when pressing space and Hook when pressing r
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jab();
        } else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Hook();
        }
    }

    public void Jab()
    {
        if (!canPunch) return; // Prevent spamming during cooldown

        canPunch = false;
        animator.SetTrigger("jab");

        float distance = Vector3.Distance(transform.position, enemy.position);
        if (distance <= punchRange)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                SoundManager.instance.PlaySoundFXClip(jabSFX, transform, 1f);
                enemyController.TakeDamage(jabDamage);
                Debug.Log("Player punched enemy!");
            }
        }

        // Start cooldown based on animation length
        StartCoroutine(PunchCooldown());
    }

    public void Hook()
    {
        if (!canPunch) return; // Prevent spamming during cooldown

        canPunch = false;
        animator.SetTrigger("hook");

        float distance = Vector3.Distance(transform.position, enemy.position);
        if (distance <= punchRange)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                SoundManager.instance.PlaySoundFXClip(hookSFX, transform, 1f);
                enemyController.TakeDamage(hookDamage);
                Debug.Log("Player punched enemy!");
            }
        }

        // Start cooldown based on animation length
        StartCoroutine(PunchCooldown());
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Player health: " + health);

        if (health <= 0)
        {
            Debug.Log("Player died!");
            Destroy(gameObject);
        }
    }

    public float GetHealth()
    {
        return health;
    }

    private IEnumerator PunchCooldown()
    {
        yield return new WaitForSeconds(punchCooldown);
        canPunch = true;
    }


}
