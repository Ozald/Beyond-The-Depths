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
    public Animator fadeAnimator;

    void Start()
    {
        fadeAnimator = Fade.instance.GetComponent<Animator>();
        yourButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        fadeAnimator.SetTrigger("Transition");

        // Wait for animation to finish
        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadScene(sceneName);
    }

}