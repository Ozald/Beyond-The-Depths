using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;



public class BackButton : MonoBehaviour
{
    // change the scene name later once we have it.
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
        mainMenu.gameObject.SetActive(true);
        settings.gameObject.SetActive(false);
    }
}