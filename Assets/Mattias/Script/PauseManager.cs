using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PauseManager : NetworkBehaviour
{
    public GameObject menuUI;

    private bool isPaused = false;

    void Update()
    {
        //Works for BOTH host and client
        if (!NetworkManager.Singleton.IsClient) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseServerRpc();
        }
    }

    //Toggle pause from client - server
    [ServerRpc(RequireOwnership = false)]
    private void TogglePauseServerRpc()
    {
        isPaused = !isPaused;
        TogglePauseClientRpc(isPaused);
    }

    //Apply pause to everyone
    [ClientRpc]
    private void TogglePauseClientRpc(bool pauseState)
    {
        isPaused = pauseState;

        if (menuUI != null)
            menuUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    //Resume button
    public void ResumeGame()
    {
        TogglePauseServerRpc();
    }

    //Restart button
    //public void RestartGame()
    //{
    //    RestartGameServerRpc();
    //}

    [ServerRpc(RequireOwnership = false)]
    private void RestartGameServerRpc()
    {
        RestartGameClientRpc();
    }

    [ClientRpc]
    private void RestartGameClientRpc()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Quit button
    public void QuitGame()
    {
        QuitGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void QuitGameServerRpc()
    {
        QuitGameClientRpc();
    }

    [ClientRpc]
    private void QuitGameClientRpc()
    {
        Debug.Log("Game Quit");

        Time.timeScale = 1f;

        //Shutdown network 
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in Unity
#else
        Application.Quit(); // Closes build
#endif
    }
}