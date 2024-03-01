using TMPro;
using UnityEngine;
/*
public class JoinCodeShow : MonoBehaviour
{
    public TextMeshProUGUI joinCodeText;
    private HostGameManager hostGameManager;

    public async void Start()
    {
        hostGameManager = new HostGameManager();

        // Subscribe to the event
        hostGameManager.OnJoinCodeCreated += UpdateJoinCodeText;

        // Start the host and initialize the join code
        await hostGameManager.StartHostAsync();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (hostGameManager != null)
        {
            hostGameManager.OnJoinCodeCreated -= UpdateJoinCodeText;
        }
    }

    private void UpdateJoinCodeText(string code)
    {
        joinCodeText.text = code;
    }
}
*/