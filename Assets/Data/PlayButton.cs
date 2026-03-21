using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;



public class PlayButton : MonoBehaviour
{
    // change the scene name later once we have it.
    public string sceneName = "example";
    public Button yourButton;
    public Canvas mainMenu;
    public Canvas settings;

    void Start()
    {
        yourButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        SceneManager.LoadScene(sceneName);
    }
}