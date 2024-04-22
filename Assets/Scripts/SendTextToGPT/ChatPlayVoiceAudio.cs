using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatPlayVoiceAudio : MonoBehaviour
{
    [SerializeField]
    private TTSManager ttsManager;

    [SerializeField]
    private ChatWithGPT chatWithGPT;
    [SerializeField]
    private ObjImporter objImporter;

    private void Start()
    {
        objImporter.OnModelRequestSuccess += OnModelGenerationFinished;
        chatWithGPT.OnChatRequestDone += OnChatRequestDone;
    }

    private void OnModelGenerationFinished()
    {
        ttsManager.PlayText("Your object has been conjured");
        chatWithGPT.SendRequest("The last object has been generated tell me a little something about it");
    }

    private void OnChatRequestDone(string response)
    {
        Debug.Log(response);
        ttsManager.PlayText(response);
    }
}
