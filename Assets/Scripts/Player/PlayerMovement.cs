
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private float groundDrag;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float footstepDelay;
    private bool isMoving = false;
    [SerializeField] private float movementThreshold;
    private Coroutine footstepCoroutine;
    private bool isPlayingFootstep = false;

    private ConstantForce constantDownForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        constantDownForce = GetComponent<ConstantForce>();
    }

    private void Update()
    {
        MovementInput();
        SpeedControl();

        rb.linearDamping = groundDrag;

        Vector2 flatVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);

        if (flatVel.magnitude > movementThreshold && !isMoving && !isPlayingFootstep)
        {
            isMoving = true;
            footstepCoroutine = StartCoroutine(PlayFootstepSoundRandomPitch());
            isPlayingFootstep = true;
        }
        else if (flatVel.magnitude <= movementThreshold && isMoving)
        {
            isMoving = false;
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                isPlayingFootstep = false;
                audioSource.Stop();
            }
        }

        if (rb.linearVelocity.y < 0)
        {
            constantDownForce.force = new Vector3(0f, -35f, 0f);
        }
        else
        {
            constantDownForce.force = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private IEnumerator PlayFootstepSoundRandomPitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(footstepDelay);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(footstepSound);
        }
    }
}