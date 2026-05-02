using UnityEngine;
using TMPro;
using Unity.Netcode;

public class GameJoinCodeUI : MonoBehaviour
{
    public TMP_Text joinCodeText;

    void Update()
    {
        if (joinCodeText == null) return;

        if (RelayManager.Instance == null || NetworkManager.Singleton == null)
        {
            joinCodeText.text = "";
            return;
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            joinCodeText.text = "";
            return;
        }

        string code = RelayManager.Instance.currentJoinCode;

        if (string.IsNullOrEmpty(code))
        {
            joinCodeText.text = "";
        }
        else
        {
            joinCodeText.text = "Join Code: " + code;
        }
    }
}