using UnityEngine;
using UnityEngine.InputSystem; // For the new Input System

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float health = 100f;
    public float punchRange = 2f;
    public float punchDamage = 20f;

    public Transform enemy;
    private Animator animator;

    void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enemy == null) return;

        // Move toward enemy
        Vector3 direction = (enemy.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        // Punch when pressing space
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Punch();
        }
    }

    public void Punch()
    {
        animator.SetTrigger("punch");
        float distance = Vector3.Distance(transform.position, enemy.position);
        if (distance <= punchRange)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(punchDamage);
                Debug.Log("Player punched enemy!");
            }
        }
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
}
