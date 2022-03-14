using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public string species;
    public int value;
    public Color primary, secondary;

    public string ambientSound;
    public float playGap;
    public float minDistance;
    public float maxDistance;

    private IEnumerator soundPlayer;

    private void Awake()
    {
        soundPlayer = playAmbient();
        StartCoroutine(soundPlayer);
    }

    private IEnumerator playAmbient()
    {
        yield return new WaitForSeconds(playGap * Random.Range(0.2f, 1f));
        while (true)
        {
            SoundManager.instance.Play3D(ambientSound, gameObject, minDistance, maxDistance);
            yield return new WaitForSeconds(playGap * Random.Range(1f, 1.2f));
        }
    }

    private void OnDestroy()
    {
        SoundManager.instance.Stop3D(gameObject);
    }
}