using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject menuUI;

    private bool isPaused = false;

    void Update()
    {
        //Only run if this instance is a client(host = also client)
        if (!NetworkManager.Singleton.IsClient) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (menuUI != null)
            menuUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    /*  public void RestartGame()
      {
          Time.timeScale = 1f;
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }
     */
    public void QuitGame()
    {
        Debug.Log("Closing game for this player");

        Time.timeScale = 1f;

        //Disconnect THIS player only
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Stops play mode
#else
    Application.Quit(); //FULLY closes the game build
#endif
    }
}