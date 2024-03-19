using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePreview : MonoBehaviour
{
    [SerializeField] ObjImporter objImporter;
    private SpriteRenderer spriteRenderer;
    private BoxCollider boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        objImporter.OnSourceImageGenerated += (Sprite sprite) =>
        {
            spriteRenderer.sprite = sprite;
            boxCollider.size = new Vector3(sprite.texture.width, sprite.texture.height, 1);
        };
    }
}
