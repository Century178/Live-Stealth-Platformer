using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float horInput;
    [SerializeField] float speed = 5f;

    [SerializeField] float jump_force = 5f;

    [SerializeField] float platformDistance = 1.2f;

    [SerializeField] float check_radius = 0.4f;
    [SerializeField] LayerMask ground_layer;
    bool grounded;

    Collider2D[] enemiesInFront;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] Transform Ground_Check;
    [SerializeField] private CapsuleCollider2D colider;
    [SerializeField] private Animator slashingAnimator;

    private bool jumpPressed;
    private bool downPressed;
    private bool slashPressed;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horInput = Input.GetAxis("Horizontal");
        if (horInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(horInput), transform.localScale.y, transform.localScale.z);
        }

        if (Input.GetButtonDown("Up"))
        {
            jumpPressed = true;
        }

        if (Input.GetButtonDown("Down"))
        {
            downPressed = true;
        }

        //stabbing in the back
        if (Input.GetKeyDown(KeyCode.Space))
        {
            slashingAnimator.SetTrigger("slash");
            slashPressed = true;
        }
    }

    private void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(Ground_Check.position, check_radius, ground_layer);

        Vector2 inFrontOfPlayer = new Vector2(transform.position.x + (1 * Mathf.Sign(rb.velocity.x)), transform.position.y);
        enemiesInFront = Physics2D.OverlapBoxAll(inFrontOfPlayer, new Vector2(2, 1), 0, enemyLayer);

        jumpPressed = jumpPressed && grounded;
        if (jumpPressed)
        {
            rb.velocity += Vector2.up * jump_force;
            jumpPressed = false;
        }

        if (slashPressed)
        {
            foreach (Collider2D enemy in enemiesInFront)
            {
                Destroy(enemy.gameObject);
            }
            slashPressed = false;
        }

        if (downPressed)
        {
            RaycastHit2D hit = Physics2D.CircleCast(Ground_Check.position, colider.size.x * 0.5f, Vector2.down, platformDistance, ground_layer);
            if (hit)
            {
                if (hit.collider.GetComponent<PlatformEffector2D>() != null)
                    StartCoroutine(DeactivatePlatform(hit.collider));
            }
            downPressed = false;
        }

        rb.velocity = new Vector2(horInput * speed, rb.velocity.y);
    }

    IEnumerator DeactivatePlatform(Collider2D platformColider)
    {
        platformColider.enabled = false;
        rb.AddForce(Vector2.down * 3f);
        while (colider.size.y + transform.position.y > platformColider.transform.position.y)//still above
        {
            yield return null;
        }
        platformColider.enabled = true;
    }

    private void OnDrawGizmos()
    {
        if (rb != null) Gizmos.DrawWireCube(new Vector2(transform.position.x + (1 * Mathf.Sign(rb.velocity.x)), transform.position.y), new Vector2(2, 1));
        Gizmos.DrawWireSphere(Ground_Check.position, check_radius);
        Gizmos.DrawLine(Ground_Check.position, Ground_Check.position + Vector3.down * platformDistance);
        Gizmos.DrawWireSphere(Ground_Check.position + Vector3.down * platformDistance, colider.size.x * 0.5f);
    }
}
