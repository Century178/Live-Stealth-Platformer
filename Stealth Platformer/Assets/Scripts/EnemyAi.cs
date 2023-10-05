using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private float speed = 20f;


    [SerializeField] private ParticleSystem noticeEffect;


    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.4f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer) == false)
            Flip();

        rb.AddForce(transform.right * speed);

    }
    private void Flip()
    {
        transform.Rotate(Vector2.up * 180);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            noticeEffect.Play();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Player>())
            Destroy(collision.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
    }
}
