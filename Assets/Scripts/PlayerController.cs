using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float accelTime = 0.05f;
    [SerializeField] float decelTime = 0.15f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 16f;
    [SerializeField] float coyoteTime = 0.12f;
    [SerializeField] float jumpBufferTime = 0.12f;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashTime = 0.15f;
    [SerializeField] float dashCooldown = 0.5f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.12f;
    [SerializeField] LayerMask groundLayer;

    Rigidbody2D rb;
    float horizontalInput;
    float velocityXSmoothing;

    // Timers
    float coyoteCounter;
    float jumpBufferCounter;

    bool isGrounded;
    bool facingRight = true;

    // Dash
    bool isDashing;
    bool canDash = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ground check
        if (groundCheck == null)
        {
            Debug.LogError("PlayerController: groundCheck is not assigned!", this);
            isGrounded = false;
        }
        else
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        
        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        // Jump buffer countdown
        jumpBufferCounter -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        float targetVelX = horizontalInput * moveSpeed;
        
        // Use decelTime when no input, accelTime when there is input
        float timeToUse = horizontalInput != 0f ? accelTime : decelTime;
        float newVelX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelX, ref velocityXSmoothing, timeToUse);
        rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);

        // Attempt jump if buffered and allowed by coyote
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            DoJump();
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }
    }

    void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void JumpPressed()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    void JumpReleased()
    {
        // Variable jump height
        if (rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
    }

    public void MoveInput(float value)
    {
        horizontalInput = value;
    }

    // Input System callbacks (for use with the Unity Input System)
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (isDashing) return;
        Vector2 v = ctx.ReadValue<Vector2>();
        horizontalInput = v.x;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started) JumpPressed();
        else if (ctx.canceled) JumpReleased();
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started) TryDash();
    }

    public void OnJumpPressed() => JumpPressed();
    public void OnJumpReleased() => JumpReleased();
    public void OnDash() => TryDash();

    void TryDash()
    {
        if (!canDash) return;
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2((facingRight ? 1f : -1f) * dashSpeed, 0f);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void LateUpdate()
    {
        // Flip sprite based on movement
        if (horizontalInput > 0.01f && !facingRight) Flip();
        else if (horizontalInput < -0.01f && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
