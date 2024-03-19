using System;
using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceRecognitionManager : MonoBehaviour
{
    public event EventHandler OnStartRecording;
    public event EventHandler OnStopRecording;
    public event Action<string> OnRequestDone;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording = false;
    private int maxTimeSeconds = 10;
    private int audioHz = 44100;

    private void Start()
    {
        OnStartRecording += StartRecording;
        OnStopRecording += StopRecording;
    }

    private void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording(this, EventArgs.Empty);
        }
    }

    private void StartRecording(object sender, EventArgs args)
    {
        Debug.Log("Start recording");
        bool audioLoop = false;
        clip = Microphone.Start(null, audioLoop, maxTimeSeconds, audioHz);
        recording = true;
    }

    private void StopRecording(object sender, EventArgs args)
    {
        Debug.Log("Stop recording");
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    public void TriggerStartRecording()
    {
        OnStartRecording?.Invoke(this, EventArgs.Empty);
    }
    public void TriggerStopRecording()
    {
        OnStopRecording?.Invoke(this, EventArgs.Empty);
    }
    public void TriggerWord()
    {
        OnRequestDone?.Invoke("cow");
    }

    public bool IsRecording()
    {
        return recording;
    }
    private void SendRecording()
    {
        Debug.Log("Sending Recording");
        HuggingFaceAPI.AutomaticSpeechRecognition(
            bytes,
            response =>
            {
                Debug.Log(response);
                OnRequestDone?.Invoke(response);
            },
            error =>
            {
                Debug.LogError(error);
            }
        );
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}
