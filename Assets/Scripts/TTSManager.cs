using System.Collections;
using System.Collections.Generic;
using LeastSquares.Overtone;
using UnityEngine;

public class TTSManager : MonoBehaviour
{
    [SerializeField] TTSPlayer ttsPlayer;
    public async void PlayText(string text) {
        await ttsPlayer.Speak(text);
    }
    
}
