using UnityEngine;

public class RelayUI : MonoBehaviour
{
    private string joinCodeInput = "";

    void OnGUI()
    {
        if (RelayManager.Instance == null) return;

        //HOST BUTTON
        if (GUI.Button(new Rect(10, 10, 150, 40), "HOST"))
        {
            RelayManager.Instance.CreateRelay();
        }

        joinCodeInput = GUI.TextField(new Rect(10, 60, 200, 30), joinCodeInput);

        
        if (GUI.Button(new Rect(10, 100, 150, 40), "JOIN"))
        {
            RelayManager.Instance.JoinRelay(joinCodeInput);
        }

        //SHOW JOIN CODE(HOST ONLY)
        if (RelayManager.Instance.showCode)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            style.fontSize = 20;

            GUI.Label(
                new Rect(10, 150, 400, 40),
                "JOIN CODE: " + RelayManager.Instance.currentJoinCode,
                style
            );
        }
    }
}