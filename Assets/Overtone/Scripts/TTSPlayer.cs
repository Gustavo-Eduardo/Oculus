using System;
using System.Threading.Tasks;
using Assets.Overtone.Scripts;
using UnityEngine;

namespace LeastSquares.Overtone
{
    public class TTSPlayer : MonoBehaviour
    {
        public event Action<string, float> OnTTSPlayerStart;
        public TTSEngine Engine;
        public TTSVoice Voice;
        public AudioSource source;
        public async Task Speak(string text)
        {
            var audioClip = await Engine.Speak(text, Voice.VoiceModel);
            source.clip = audioClip;
            source.loop = false;
            source.Play();
            
            OnTTSPlayerStart?.Invoke(text, audioClip.length);
        }
    }
}