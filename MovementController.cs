using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    [Header("Movement")]

    [SerializeField] private Transform orientation;

    private Vector3 movement;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float moveSpeed, mouseSens;
    private float OGSpeed, OGJump, speedySpeed, rotY, rotX;
    [SerializeField] private Camera mainCamera;

    private Vector3 OGSize;



    [Header("Keybinds")]
    public KeyCode jumpKey;
    public KeyCode sprintKey;
    public KeyCode crouchKey;



    [Header("Jumping")]

    public float jumpForce;
    public float jumpCooldown;
    public float airMultipier;
    public float playerHeight;

    [SerializeField] private LayerMask ground, wall;
    private bool grounded, readyToJump = true;








    private void Start()
    {
        Application.targetFrameRate = 800;
        OGSpeed = moveSpeed;
        OGSize = transform.localScale;
        OGJump = jumpForce;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


    }
    // Update is called once per frame
    void Update()
    {
        //Movement
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        movement = orientation.right * x + orientation.forward * z;
        movement.Normalize();

        rotX = Input.GetAxis("Mouse X") * mouseSens;
        rotY -= Input.GetAxis("Mouse Y") * mouseSens;

        //RotY
        rotY = Mathf.Clamp(rotY, -80, 80);
        mainCamera.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

        //RotX
        transform.Rotate(0, rotX, 0);






        //Jumping
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (grounded)
        {
            if (rb.velocity.x > 1)
            {
                rb.velocity -= new Vector3(0.1f, 0, 0);
            }

            if (rb.velocity.z > 1)
            {
                rb.velocity -= new Vector3(0, 0, 0.1f);
            }
        }
        if (!grounded)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, OGSize, 0.02f);
        }

        Debug.Log(movement);
    }

    private void FixedUpdate()
    {
        if (grounded)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else if (!grounded)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * airMultipier * Time.fixedDeltaTime);
        }
    }






    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ResetSpeed()
    {
        moveSpeed = OGSpeed;
        jumpForce = OGJump;
    }



}
