using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    private Scrollbar scrollbar;
    private float playerHealth;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        scrollbar.size = playerController.GetHealth()/100;
    }
}
