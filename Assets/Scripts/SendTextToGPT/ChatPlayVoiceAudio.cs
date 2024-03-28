using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatPlayVoiceAudio : MonoBehaviour
{
    [SerializeField]
    private TTSManager ttsManager;

    [SerializeField]
    private ChatWithGPT chatWithGPT;

    private void Start()
    {
        chatWithGPT.OnChatRequestDone += OnChatRequestDone;
    }

    private void OnChatRequestDone(string response)
    {
        Debug.Log(response);
        ttsManager.PlayText(response);
    }
}
