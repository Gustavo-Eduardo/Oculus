using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCircleManager : MonoBehaviour
{
    [SerializeField] private ObjImporter objImporter;
    [SerializeField] private GameObject selectorPrefab;
    private GameObject currentSpawnCircle;
    // Start is called before the first frame update
    private void Start()
    {
        objImporter.OnModelGenerationStarted += OnModelGenerationStarted;
        objImporter.OnModelGenerationFinished += OnModelGenerationFinished;
    }

    private void ClearCurrentSpawnCircle()
    {
        if (currentSpawnCircle != null)
        {
            Destroy(currentSpawnCircle);
        }
    }

    private void OnModelGenerationFinished(GameObject renderedMesh) {
        ClearCurrentSpawnCircle();
    }

    private void OnModelGenerationStarted(string word, Vector3 spawnPosition)
    {
        ClearCurrentSpawnCircle();
        GameObject selector = Instantiate(selectorPrefab);
        currentSpawnCircle = selector;
        selector.transform.position = spawnPosition - Vector3.up + Vector3.up * 0.01f;
    }
}
