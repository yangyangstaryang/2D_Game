using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    private float inputX;
    private bool onGround;
    private int currentJumpCount;
    private bool isJump;
    
    public Rigidbody2D rb;
    public float jumpForce;
    public float moveSpeed = 5f;
    public LayerMask groundLayer;
    public int jumpCount = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpCount = jumpCount;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
        onGround = rb.IsTouchingLayers(groundLayer);

        if (onGround && rb.velocity.y == 0)
        {
            currentJumpCount =  jumpCount;
            isJump = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount > 0)
        {
            isJump = true;
            currentJumpCount--;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
