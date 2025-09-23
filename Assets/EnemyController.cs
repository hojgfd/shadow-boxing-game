using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;

    private Animator animator;
    public Transform player;
    

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player != null)
        {
            // Make this object face the target
            transform.LookAt(player);
        }
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
