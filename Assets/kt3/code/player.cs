using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator anim;

    [SerializeField] private bool canDoubleJump = true;
    [SerializeField] private int maxJumps = 2;

    private float coyoteTimeCounter;
    private Vector3 _position;
    private Rigidbody2D rb;
    private int jumpsRemaining;
    private bool isGrounded;
    private bool jumpRequested;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        Vector2 moveDirection = new Vector2(_position.x, 0);
        transform.position += (Vector3)moveDirection * (_speed * Time.deltaTime);

        HandleCoyoteTime();
        CheckGrounded();

        if (jumpRequested)
        {
            TryJump();
            jumpRequested = false;
        }
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
            anim.SetBool("up", false);
        }
    }

    void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _position = new Vector2(input.x, 0);
        if (context.performed)
        {
            anim.SetBool("go", true);
        }
        else
            anim.SetBool("go", false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            anim.SetBool("up", true);
            jumpRequested = true;
        }
    }

    void TryJump()
    {
        bool canJumpFromGround = coyoteTimeCounter > 0 && jumpsRemaining == maxJumps;
        bool canDoubleJump = jumpsRemaining > 0;
        print(isGrounded);
        if (jumpsRemaining > 0 || coyoteTimeCounter > 0)
        {
            Jump();
            jumpsRemaining--;
            //if (!canJumpFromGround) 
            //{
            //}
            //else 
            //{
            //    jumpsRemaining = maxJumps - 1;
            //    coyoteTimeCounter = 0; 
            //}
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        Debug.Log($"Прыжок! Осталось прыжков: {jumpsRemaining}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * groundCheckDistance;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireSphere(endPos, 0.05f);
    }

    public void Animator()
    {
        if (rb.linearVelocity.x >= 0.1f)
            anim.SetBool("go", true);
        else
            anim.SetBool("go", false);
    }
}