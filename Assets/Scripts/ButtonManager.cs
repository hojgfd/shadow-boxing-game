using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ButtonManager : MonoBehaviour
{
    public Button playButton;
    public Button tutorialButton;


    void Start()
    {
        // Add listeners to the buttons
        playButton.onClick.AddListener(OnPlayButtonClicked);
        tutorialButton.onClick.AddListener(OnTutorialButtonClicked);
    }



    void OnPlayButtonClicked()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("SampleScene");
    }

    void OnTutorialButtonClicked()
    {
        SceneManager.LoadScene("Tutorial");
    }

}
