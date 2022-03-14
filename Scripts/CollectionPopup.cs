using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectionPopup : MonoBehaviour
{
    public float moveSpeed;
    public float lifespan;
    public float fadeSpeed;
    private TextMeshPro textMesh;
    private Color textColor, textOutline;

    void Update()
    {
        transform.position += new Vector3(0, moveSpeed) * Time.deltaTime;

        lifespan -= Time.deltaTime;
        if (lifespan < 0)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            textOutline.a -= fadeSpeed * Time.deltaTime;
            textMesh.faceColor = textColor;
            textMesh.outlineColor = textOutline;

            if (textColor.a < 0)
                Destroy(gameObject);
        }
    }

    public void Setup(int value, Color primary, Color secondary)
    {
        textMesh = GetComponent<TextMeshPro>();

        string text = value.ToString();
        if (value >= 0)
            text = "+" + text;

        textMesh.SetText(text);
        textColor = primary;
        textColor.a = 1;
        textOutline = secondary;
        textOutline.a = 1;

        textMesh.faceColor = textColor;
        textMesh.outlineColor = textOutline;
    }
}
