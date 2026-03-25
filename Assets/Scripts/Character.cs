using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Character : MonoBehaviour, IMovement
{
    [SerializeField] float speed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 3f;
    [SerializeField] float dashCooldownTimer = 0f;
    [SerializeField] float dashCounts;
    [SerializeField] float maxDashCounts = 3;
    [SerializeField] float gravity = -9.81f;

    public bool isPlayer2;
    CharacterController controller;
    Vector3 playerVelocity;
    float dashTimer = 0f;
    Vector3 dashDirection = Vector3.zero;
    bool isDashing = false;

    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        dashCounts = maxDashCounts;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayer2)
        {
            MoveControl(Input.GetAxis("HorizontalPlayer1"), Input.GetAxis("VerticalPlayer1"),Input.GetKeyDown(KeyCode.G));
        }
        else
        {
            MoveControl(Input.GetAxis("HorizontalPlayer2"), Input.GetAxis("VerticalPlayer2"),Input.GetKeyDown(KeyCode.Keypad2));
        }
        
    }
    public void Move(Vector3 direction)
    {
        controller.Move(direction * speed * Time.deltaTime);
    }
    public void Dash(Vector3 direction)
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTimer = 0f;
            dashDirection = direction.normalized;
        }
    }
    public void MoveControl(float xInp, float zInp, bool isDashInput)
    {
        Vector3 move;

        if(controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;//reset gravity when grounded
        }
        playerVelocity.y += gravity * Time.deltaTime;//gravity
        move = transform.right * xInp + transform.forward * zInp;
        Vector3 direction = move + Vector3.up * playerVelocity.y;//final move

        if(isDashInput)//dash input
        {
            if(dashCounts>0)
            {
                --dashCounts;
                Dash(direction);
            }
        }

        if(dashCounts < maxDashCounts){
            dashCooldownTimer += Time.deltaTime;
            if(dashCooldownTimer >= dashCooldown)
            {
                dashCounts = maxDashCounts;
                dashCooldownTimer = 0f;
            }
        }

        if(isDashing)//during dashing
        {
            dashTimer += Time.deltaTime;
            float t = Mathf.Clamp01(dashTimer / dashDuration);
            controller.Move(Vector3.Lerp(Vector3.zero, dashDirection * dashSpeed * Time.deltaTime, t));
            isDashing = dashTimer < dashDuration;
        }
        else
        {
            Move(direction);
        }
    }
    
    
}
