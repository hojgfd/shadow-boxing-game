using UnityEngine;
using System.Collections; // Nødvendig for Coroutines (cooldown)

// Dette script styrer fjendens AI, bevægelse og skadeshåndtering.
// Den er baseret på din PlayerController for at sikre kompatibilitet i spillet.
public class EnemyController : MonoBehaviour
{
    // === Fjendens Stats (Kan justeres i Unity Inspector) ===
    [Header("Enemy Stats")]
    [SerializeField] private float health = 100f; 
    [SerializeField] private float moveSpeed = 4.5f; 
    
    [Header("Punch Settings")]
    [SerializeField] private float punchDamage = 15f; 
    [SerializeField] private float punchRange = 2f; // Samme rækkevidde som Player
    [SerializeField] private float punchCooldown = 1.5f; // Hvor ofte fjenden kan slå

    // === Animation Settings ===
    // Sæt denne trigger i din Unity Animator Component for Enemy
    // Vi bruger "EnemyPunch" som standard
    [SerializeField] private string enemyPunchTriggerName = "EnemyPunch"; 
    
    // === AI State og Referencer ===
    private Animator animator;
    public Transform player; // Bliver fundet i Start()
    private Rigidbody rb;
    private bool canPunch = true; 
    
    void Start()
    {
        // Giver fjenden en anden farve, så du kan se forskel
        GetComponent<Renderer>().material.color = Color.red;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();


        // Finder Playeren automatisk via tag (Kræver, at Player GameObject har tagget "Player")
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        } 
        else
        {
            Debug.LogError("Fejl: Kunne ikke finde et GameObject med tagget 'Player'. Fjenden kan ikke bevæge sig!");
        }     
        Physics2D.IgnoreCollision(playerObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    // AI'ens 'hjerne' kører i hvert frame
    void Update()
    {
        if (player == null || health <= 0) return; 

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // 1. Beregn retning og se på Playeren (samme som i din PlayerController)
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);

        // 2. BEVÆGELSE: Bevæg dig mod Playeren, hvis den er for langt væk (samme logik som din Player)
        if (distanceToPlayer > punchRange - 0.5f)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        // 3. KAMP LOGIK: Angrib automatisk, hvis Player er i rækkevidde og cooldown er klar
        if (distanceToPlayer <= punchRange && canPunch)
        {
            Punch(); 
        }

        Debug.Log(health);
    }
    
    // === Fjendens Angrebsmetode ===
    public void Punch()
    {
        if (!canPunch) return; 

        canPunch = false;
        
        // Brug den definerede animationstrigger
        if (animator != null)
        {
            animator.SetTrigger(enemyPunchTriggerName); 
        }

        // Tjek om Playeren er i rækkevidde
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= punchRange)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Fjenden slår Playeren
                // OBS: Vi kan IKKE spille SoundFX her, da vi ikke har SoundManager.cs
                playerController.TakeDamage(punchDamage); 
                Debug.Log("Enemy har slået Player!");
            }
        }

        // Start cooldown
        StartCoroutine(PunchCooldownRoutine());
    }

    private IEnumerator PunchCooldownRoutine()
    {
        yield return new WaitForSeconds(punchCooldown);
        canPunch = true; 
    }
    
    // === Skadeslogik (Dette kaldes fra PlayerController) ===
    public void TakeDamage(float amount)
    {
        if (health <= 0) return; 
        
        health -= amount;
        Debug.Log("Enemy health: " + health);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Du kan tilføje en "Hit" animationstrigger her
            // f.eks. if (animator != null) animator.SetTrigger("Hit"); 
        }
    }

    private void Die()
    {
        Debug.Log("Enemy døde!");
        Destroy(gameObject); 
    }
}
