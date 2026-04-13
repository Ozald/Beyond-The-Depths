using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas settings;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        pauseMenu.gameObject.SetActive(true);
        settings.gameObject.SetActive(false); 
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Resume()
    {
        pauseMenu.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
    }
}