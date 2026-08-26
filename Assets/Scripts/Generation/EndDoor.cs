using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDoor : MonoBehaviour
{

    public Room parentRoom;
    public bool enabled = false;
    public Animator fadeAnimator;

    // There was a bug that cause the door transitions to loop, this fixes that
    public bool isTransitioning;

    void Start()
    {
        fadeAnimator = Fade.instance.GetComponent<Animator>();
        isTransitioning = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && enabled)
        {
            Debug.Log("Player has entered a door");

            SaveManager.SaveData();
            StartCoroutine(FadeTransition(other));
        }
    }

    private IEnumerator FadeTransition(Collider2D other)
    {
        fadeAnimator.SetTrigger("FadeOut");

        // Wait for animation to finish
        yield return new WaitForSecondsRealtime(1f);
        AudioManager.instance.currentBGMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}

