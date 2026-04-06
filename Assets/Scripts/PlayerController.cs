using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    private int jumpsRemaining;
    private bool wasGrounded;
    public float speed = 0;
    public float jumpForce = 7f;
    public int maxJumps = 2;
    public float groundCheckDistance = 0.55f;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    void Start()
    {
        jumpsRemaining = maxJumps;
        count = 0;
        rb = GetComponent<Rigidbody>();
        SetCountText();
        winTextObject.SetActive(false);
    }

    void OnMove (InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump(InputValue value)
    {
        if (!value.isPressed || jumpsRemaining <= 0)
            return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpsRemaining--;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            ~0,
            QueryTriggerInteraction.Ignore);
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 4)
        {
            winTextObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        // Only refill jumps when we *land*, not every frame the ray hits the floor.
        // Otherwise the ray still reports grounded for a few frames after takeoff and you get infinite jumps.
        bool grounded = IsGrounded();
        bool settledOnGround = grounded && rb.linearVelocity.y <= 0.05f;
        if (settledOnGround)
        {
            if (!wasGrounded)
                jumpsRemaining = maxJumps;
            wasGrounded = true;
        }
        else
            wasGrounded = false;

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }
}
