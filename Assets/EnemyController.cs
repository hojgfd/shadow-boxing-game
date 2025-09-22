using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;

    private Animator animator;

    void Start()
    {
        GetComponent<Renderer>().material.color = Color.red;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // You could add enemy AI here (e.g., chase player).
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Enemy health: " + health);

        if (health <= 0)
        {
            Debug.Log("Enemy died!");
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            animator.SetTrigger("punch");
            Debug.Log("Enemy hit player!");
        }
    }
}
