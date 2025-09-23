using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TutorialManager : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
