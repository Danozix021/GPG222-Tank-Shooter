using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    string joinCode = "";

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 40), "HOST"))
        {
            RelayManager.Instance.CreateRelay();
        }

        joinCode = GUI.TextField(new Rect(10, 60, 200, 30), joinCode);

        if (GUI.Button(new Rect(10, 100, 150, 40), "JOIN"))
        {
            RelayManager.Instance.JoinRelay(joinCode);
        }
    }
}