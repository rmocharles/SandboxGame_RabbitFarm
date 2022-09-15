using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsOnline : MonoBehaviour
{
    public Text isOnlineText;
    string nowState = "오프라인";

    // Update is called once per frame
    void Update()
    {
        isOnlineText.text = nowState;
    }

    public void SetIsOnline(string state)
    {
        Debug.LogError(state);
        nowState = state;
    }
}
