using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatPlayVoiceAudio : MonoBehaviour
{
    [SerializeField]
    private TTSManager ttsManager;

    [SerializeField]
    private SendTextToGPT sendTextToGPT;

    private void Start()
    {
        sendTextToGPT.OnChatRequestDone += OnChatRequestDone;
    }

    private void OnChatRequestDone(string response)
    {
        Debug.Log(response);
        ttsManager.PlayText(response);
    }
}
