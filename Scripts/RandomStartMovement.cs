using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStartMovement : MonoBehaviour
{
    public float maxStartVelocity;
    public float maxStartRotation;

    public float fleeSpeed;
    public float fleeAcceleration;

    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.velocity = new Vector2(Random.Range(-maxStartVelocity, maxStartVelocity), Random.Range(-maxStartVelocity, maxStartVelocity));
        rb.angularVelocity = Random.Range(-maxStartRotation, maxStartRotation);
    }

    public void Flee(Vector2 playerPos, Vector2 playerVelocity)
    {
        anim.SetBool("flee", true);

        Vector2 alienPos = transform.position;
        Vector2 targetVelocity = (alienPos - playerPos).normalized * fleeSpeed;

        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, fleeAcceleration * Time.deltaTime);
    }

    public void StopFlee()
    {
        anim.SetBool("flee", false);
    }
}
