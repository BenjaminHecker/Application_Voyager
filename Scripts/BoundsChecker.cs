using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoundsChecker : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;

    private void Awake()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        gameObject.DrawCircle(collider.radius, 0.2f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PlayerInventory.EndSession();
            SoundManager.instance.Play("Boundary");
            SceneManager.LoadScene(3, LoadSceneMode.Single);
        }
    }
}
