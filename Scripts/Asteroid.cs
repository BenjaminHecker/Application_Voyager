using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public int value;
    public Color primary, secondary;

    public bool varySprite;

    [SerializeField] List<Sprite> sprites;

    private void Awake()
    {
        if (varySprite)
        {
            SpriteRenderer sRender = GetComponent<SpriteRenderer>();
            sRender.sprite = sprites[Random.Range(0, sprites.Capacity)];
        }
    }
}