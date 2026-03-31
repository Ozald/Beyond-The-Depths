using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;



public class SettingsButtonPauseMenu : MonoBehaviour
{
    // change the scene name later once we have it.
    public Button yourButton;
    public Canvas pauseMenu;
    public Canvas settings;

    void Start()
    {
        yourButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        pauseMenu.gameObject.SetActive(false);
        settings.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
}