using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float horInput;
    [SerializeField] float speed = 5f;

    [SerializeField] float jump_force = 5f;
    [SerializeField] float maximumYVelocity = 0.5f;

    [SerializeField] float platformDistance = 1.2f;

    [SerializeField] float check_radius = 0.4f;
    [SerializeField] LayerMask ground_layer;
    bool grounded;

    Collider2D[] enemiesInFront;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] Transform Ground_Check;
    private Rigidbody2D rb;
    [SerializeField] private CapsuleCollider2D colider;
    [SerializeField] private Animator slashingAnimator;

    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(Ground_Check.position, check_radius, ground_layer);
        horInput = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(horInput * speed, rb.velocity.y);

    }

    void Update()
    {
        Vector2 inFrontOfPlayer = new Vector2(transform.position.x + (1 * Mathf.Sign(rb.velocity.x)), transform.position.y);
        enemiesInFront = Physics2D.OverlapBoxAll(inFrontOfPlayer, new Vector2(2, 1), 0, enemyLayer);

        if (Input.GetButtonDown("Up") && grounded && Mathf.Abs(rb.velocity.y) < maximumYVelocity)
        {
            rb.velocity += Vector2.up * jump_force;
        }

        if (Input.GetButtonDown("Down"))
        {
            RaycastHit2D hit = Physics2D.CircleCast(Ground_Check.position, colider.size.x * 0.5f, Vector2.down, platformDistance, ground_layer);
            if (hit.point != Vector2.zero)
            {
                if (hit.collider.GetComponent<PlatformEffector2D>() != null)
                    StartCoroutine(DeactivatePlatform(hit.collider));
            }

        }
        //stabbing in the back
        if (Input.GetKeyDown(KeyCode.Space))
        {
            slashingAnimator.SetTrigger("slash");
            foreach (Collider2D enemy in enemiesInFront)
            {
                if (enemy.transform.localEulerAngles.y < 5 && facingRight == true)//you are both looking right
                    Destroy(enemy.gameObject);
                else if (enemy.transform.localEulerAngles.y > 170 && facingRight == false)//you are both looking left
                    Destroy(enemy.gameObject);
            }
        }

        if (horInput > 0 && facingRight == false)// going right but facing left
            Flip();
        else if (horInput < 0 && facingRight == true) // going left but looking right
            Flip();
    }
    IEnumerator DeactivatePlatform(Collider2D platformColider)
    {
        platformColider.enabled = false;
        rb.AddForce(Vector2.down * 3f);
        while (colider.size.y + transform.position.y > platformColider.transform.position.y)//still above
        {
            yield return new WaitForSeconds(0.05f);// wait a bit
        }
        platformColider.enabled = true;
    }

    private void Flip()
    {
        transform.Rotate(Vector2.up * 180);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Ground_Check.position, check_radius);
        Gizmos.DrawLine(Ground_Check.position, Ground_Check.position + Vector3.down * platformDistance);
        Gizmos.DrawWireSphere(Ground_Check.position + Vector3.down * platformDistance, colider.size.x * 0.5f);
    }
}
