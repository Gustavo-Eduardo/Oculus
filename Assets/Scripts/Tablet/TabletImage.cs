using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TabletImage : MonoBehaviour
{
    [SerializeField] private Image PreviewImage;

    public string OutputURL;

    public void SetUpImage(Sprite sprite)
    {
        PreviewImage.sprite = sprite;
    }

    public void LoadObject()
    {
        Debug.Log("Clickkkkk");
        StartCoroutine(LoadObjectCoroutine());
    }

    private IEnumerator LoadObjectCoroutine()
    {
        string meshUrl = OutputURL;
        UnityWebRequest www = UnityWebRequest.Get(meshUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API request failed: " + www.error);
            yield break;
        }

        ObjImporter.Instance.ImportGLB(www.downloadHandler.data, meshUrl, SpawnRandomObject.spawnPosition);
    }
}