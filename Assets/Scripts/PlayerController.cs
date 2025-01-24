using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 15f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private int jumpCount;

    [Header("Jump Settings")]
    public int maxJumps = 2; // Max number of jumps (e.g., double jump)

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimatorParameters();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Flip character based on movement direction
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1); // Facing right
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Facing left
    }

    private void HandleJump()
    {
        // Allow jump if grounded or if jumpCount is less than maxJumps
        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount < maxJumps))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
            jumpCount++; // Increment the jump count
        }
    }

    private void CheckGrounded()
    {
        // Check if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset jump count when grounded
        if (isGrounded)
        {
            jumpCount = 0; // Reset jump count
        }
    }

    private void UpdateAnimatorParameters()
    {
        // Update xVelocity based on horizontal speed
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

        // Update yVelocity based on vertical speed
        animator.SetFloat("yVelocity", rb.velocity.y);

        // Update isJumping based on grounded status
        animator.SetBool("IsJumping", !isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check radius for debugging
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
