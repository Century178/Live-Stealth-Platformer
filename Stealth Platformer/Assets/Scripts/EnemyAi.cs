using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    [SerializeField] private ParticleSystem noticeEffect;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.4f;

    [SerializeField] private float wallCheckDistance = 0.9f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer) == false ||
            Physics2D.Raycast(transform.position, transform.right, wallCheckDistance, groundLayer))
        {
            Flip();
        }

        rb.AddForce(transform.right * speed);
    }

    private void Flip()
    {
        transform.Rotate(Vector2.up * 180);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) InitiateLoss();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) InitiateLoss();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * wallCheckDistance);
    }

    private void InitiateLoss()
    {
        noticeEffect.Play();
        Time.timeScale = 0;
        StartCoroutine(Lose());
    }

    private IEnumerator Lose()
    {
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
