using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TabletController : MonoBehaviour
{
    [SerializeField] private Transform imageButtonTemplate;
    [SerializeField] private Transform imagePanel;

    private void Start()
    {
        StartCoroutine(LoadModelPreviews());
    }

    private IEnumerator LoadModelPreviews()
    {
        // Getting List of available converted models
        string conversionsUrl = "http://52.53.194.213:8000/api/conversions/";

        UnityWebRequest conversionsRequest = UnityWebRequest.Get(conversionsUrl);

        string bearerToken = "8606e97a3516fb6f5ecefdf1db91bc286333ce14";
        conversionsRequest.SetRequestHeader("Authorization", "Token " + bearerToken);

        yield return conversionsRequest.SendWebRequest();

        Debug.Log($"result: {conversionsRequest.result}");
        if (conversionsRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed retrieving conversions: " + conversionsRequest.error);
            yield break;
        }

        string jsonText = conversionsRequest.downloadHandler.text;
        ConversionData[] conversionDataArray = JsonHelper.FromJson<ConversionData>(jsonText);

        foreach (var data in conversionDataArray)
        {
            string sourceImageUrl = data.source_file;
            Debug.Log($"url {sourceImageUrl}");
            UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(sourceImageUrl);
            yield return imageRequest.SendWebRequest();

            if (imageRequest.result == UnityWebRequest.Result.Success)
            {
                DownloadHandlerTexture downloadHandlerTexture =
                    imageRequest.downloadHandler as DownloadHandlerTexture;

                Texture2D texture = downloadHandlerTexture.texture;

                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f
                );

                Transform imageTransform = Instantiate(imageButtonTemplate, imagePanel);
                imageTransform.gameObject.SetActive(true);

                TabletImage currentImage = imageTransform.GetComponent<TabletImage>();
                currentImage.SetUpImage(sprite);
                currentImage.OutputURL = data.output_file;
            }
        }
    }

    [Serializable]
    public class ConversionData
    {
        public string identifier;
        public string output_file;
        public string source_file;
        public string started_at;
        public string finished_at;
        public string duration;
    }
}