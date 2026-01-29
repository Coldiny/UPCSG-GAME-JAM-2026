using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController2D controller;
    public TrailRenderer tr;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public Animator anim;
    public GameObject DashUI;
    public GameObject DoubleJumpUI;
    PlayerHealth playerHealth;

    [Header("Ability Unlocks")]
    public bool canDoubleJump = false; // Check this ONLY when player gets the item!
    public bool canDash = false;        // Option to unlock dashing later too
    public bool canSprint = true;      // Option to unlock sprinting later too

    [Header("Movement Settings")]
    public float CharacterSpeed = 40f;
    private float defaultSpeed = 40f;

    [Header("Jump Settings")]
    public float doubleJumpForce = 25f;
    public int extraJumpsValue = 1;
    private int extraJumps;
    private bool isGrounded;
    private bool doDoubleJump;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    // Internal State
    [SerializeField] float horizontalMove = 0f;
    [SerializeField] bool jump = false;
    [SerializeField] bool isDashing = false;
    [SerializeField] bool isSprinting = false;
    private bool dashReady = true; // Renamed from canDash to avoid confusion

    private void Start()
    {
        defaultSpeed = CharacterSpeed;
        extraJumps = extraJumpsValue;
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (playerHealth.IsDead)
        {
            enabled = false; // disables input code?
            rb.linearVelocity = Vector2.zero;
            return;
        }


        UpdateDashUI();
        UpdateDoubleJump();
        // --- 1. Ground Check ---
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, whatIsGround);
        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        // --- 2. Movement Inputs ---
        float moveInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
        }

        horizontalMove = moveInput * CharacterSpeed;
        anim.SetFloat("Speed", Mathf.Abs(horizontalMove));

        // --- 3. Sprint Logic ---
        // Added 'canSprint' check here in case you want to unlock that too
        if (canSprint && Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
        {
            isSprinting = true;
            CharacterSpeed = 60f;
            anim.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }
        else
        {
            isSprinting = false;
            CharacterSpeed = defaultSpeed;
        }



        // --- 4. Jump & Double Jump Logic ---
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isGrounded)
            {
                jump = true; // Normal Jump
            }
            // CHECK: Do we have jumps left? AND Do we have the ability unlocked?
            else if (extraJumps > 0 && canDoubleJump)
            {
                doDoubleJump = true;
                extraJumps--;
            }
        }

        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        // --- 5. Dash Logic ---
        if (isDashing) return;

        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame && dashReady && canDash)
        {
            StartCoroutine(Dash());
        }

    }

    private void FixedUpdate()
    {
        

        if (playerHealth.IsDead)
        {
            enabled = false; // disables input code?
            rb.linearVelocity = Vector2.zero;
            return;
        }


        if (isDashing) return;

        if (doDoubleJump)
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
            doDoubleJump = false;
            jump = false;
        }

        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    private IEnumerator Dash()
    {
        dashReady = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float direction = transform.localScale.x > 0 ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        if (tr != null) tr.emitting = true;

        yield return new WaitForSeconds(dashTime);

        if (tr != null) tr.emitting = false;

        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashReady = true;
    }

    public void UpdateDashUI()
    {
        if(dashReady && canDash)
        {
            DashUI.SetActive(true);
        }
        else
        {
            DashUI.SetActive(false);
        }
    }

    public void UpdateDoubleJump()
    {
        if (extraJumps > 0 && canDoubleJump)
        {
            DoubleJumpUI.SetActive(true);
        }
        else
        {
            DoubleJumpUI.SetActive(false);
        }
    }
}