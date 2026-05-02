using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeText;
    public TMP_Text statusText;

    private void Update()
    {
        if (RelayManager.Instance == null) return;

        if (RelayManager.Instance.showCode)
        {
            joinCodeText.text = "Join Code: " + RelayManager.Instance.currentJoinCode;
        }
        else
        {
            joinCodeText.text = "";
        }
    }

    public async void HostLobby()
    {
        statusText.text = "Creating lobby";

        if (RelayManager.Instance != null)
        {
            await RelayManager.Instance.CreateRelay();
            statusText.text = "Lobby hosted.";

            Unity.Netcode.NetworkManager.Singleton.SceneManager.LoadScene("Daniel",UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public void JoinLobby()
    {
        if (RelayManager.Instance == null) return;

        string code = joinCodeInput.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            statusText.text = "Please enter a join code.";
            return;
        }

        statusText.text = "Joining lobby";
        RelayManager.Instance.JoinRelay(code);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}