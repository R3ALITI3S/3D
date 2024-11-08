using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
   // Variables
   [SerializeField] private float moveSpeed;
   [SerializeField] private float walkSpeed;
   [SerializeField] private float runSpeed;

   [SerializeField] private bool isGrounded;
   [SerializeField] private float groundCheckDistance;
   [SerializeField] private LayerMask groundMask;
   [SerializeField] private float gravity;
   
   [SerializeField] private float jumpHeight;
   
   private Vector3 moveDirection;
   private Vector3 velocity;
   
   // References
   private CharacterController controller;
   private Animator anim;

   // New jump control variables
   private bool isJumping = false; // Ensures jump only triggers once until landing

   private void Start()
   {
      controller = GetComponent<CharacterController>();
      anim = GetComponentInChildren<Animator>();
   }

   private void Update()
   {
      Move();
   }

   private void Move()
   {
      // Check if grounded
      isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

      // Update the "IsJumping" parameter based on grounded state
      anim.SetBool("IsJumping", !isGrounded);

      // Reset jump status when grounded
      if (isGrounded)
      {
         if (isJumping && velocity.y < 0) // Only reset when actually landing
         {
            isJumping = false;
         }
         velocity.y = -2f;
      }

      // Get input for forward/backward and left/right movement
      float moveZ = Input.GetAxis("Vertical");   // For forward and backward movement
      float moveX = Input.GetAxis("Horizontal"); // For left and right movement
      moveDirection = new Vector3(moveX, 0, moveZ).normalized; // Normalize to prevent faster diagonal movement

      if (isGrounded)
      {
         if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
         {
            Run();
         }
         else if (moveDirection != Vector3.zero)
         {
            Walk();
         }
         else
         {
            Idle();
         }
         
         moveDirection *= moveSpeed;
         
         // Check for jump input, only allow if grounded and not already jumping
         if(Input.GetKeyDown(KeyCode.Space) && !isJumping)
         {
            Jump();
         }
      }

      // Move the player in the calculated direction and apply gravity
      Vector3 move = transform.TransformDirection(moveDirection); // Convert local to world space
      controller.Move(move * Time.deltaTime);
      
      velocity.y += gravity * Time.deltaTime;
      controller.Move(velocity * Time.deltaTime);
   }

   private void Idle()
   {
      anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
   }
   
   private void Walk()
   {
      moveSpeed = walkSpeed;
      anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
   }
   
   private void Run()
   {
      moveSpeed = runSpeed;
      anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
   }

   private void Jump()
   {
      velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
      anim.SetBool("IsJumping", true); // Set jumping animation
      isJumping = true; // Prevent re-triggering jump until grounded
   }
}
