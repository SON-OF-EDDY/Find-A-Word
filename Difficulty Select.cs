using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelect : MonoBehaviour
{
    public static DifficultySelect Instance; // Singleton instance
    public string difficulty = "Easy";
    public AudioSource clickSFX;
    public GameObject loadingWheel;
    public GameObject loadingText;
    public Animator transitionAnimator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }




    private IEnumerator LoadSceneAsync(string sceneName)
    {
        clickSFX.Play();
        Debug.Log("Starting loading animation...");
        loadingWheel.SetActive(true);
        loadingText.SetActive(true);

        float minimumLoadingTime = 2.0f; // Set a minimum loading time of 2 seconds
        float startTime = Time.time; // Record the time when the loading starts

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Prevent the scene from activating immediately

        // Wait for either the minimum time to pass or the scene to finish loading
        while (!asyncLoad.isDone)
        {
            // Calculate how long we've been waiting
            float elapsedTime = Time.time - startTime;

            // Check if the scene is ready to be activated and minimum time has passed
            if (asyncLoad.progress >= 0.90f && elapsedTime >= minimumLoadingTime)
            {
                // Trigger the "start" animation
                transitionAnimator.SetTrigger("start");

                // Wait for the transition animation to finish before loading the scene
                yield return new WaitForSeconds(2f); // Adjust this to match your animation length

                // Activate the scene
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Stopping loading animation...");
        //loadingWheel.SetActive(false);
        //loadingText.SetActive(false);
    }


    public void GoToPlayScene(string name)
    {

        StartCoroutine(LoadSceneAsync(name));
    }

    public void EndGame()
    {
        Debug.Log("Closing the game now...");
        Application.Quit();

    }


    public void setDifficultyEasy ()
    {

        DifficultySelect.Instance.difficulty = "Easy";
        //Debug.Log("Difficult set to: " + difficulty);
        Debug.Log("EASY mode button clicked");
        //SceneManager.LoadScene("SampleScene");
        //StartCoroutine(LoadSceneAsync("SampleScene"));
        GoToPlayScene("SampleScene");

    }

    public void setDifficultyMedium()
    {
        DifficultySelect.Instance.difficulty = "Medium";
        //Debug.Log("Difficult set to: " + difficulty);
        Debug.Log("mEDIUM mode button clicked");
        //SceneManager.LoadScene("SampleScene");
        //StartCoroutine(LoadSceneAsync("SampleScene"));
        GoToPlayScene("SampleScene");
    }

    public void setDifficultyHard()
    {
        DifficultySelect.Instance.difficulty = "Hard";
        //Debug.Log("Difficult set to: " + difficulty);
        Debug.Log("HARD mode button clicked");
        //SceneManager.LoadScene("SampleScene");
        //StartCoroutine(LoadSceneAsync("SampleScene"));
        GoToPlayScene("SampleScene");
    }
}
