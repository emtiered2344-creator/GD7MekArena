using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Character : MonoBehaviour, IMovement
{
    public bool isPlayer2;
    [Header("Movement Stat")]
    [SerializeField] float speed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] float dashCooldownTimer;
    [SerializeField] float dashCounts;
    [SerializeField] float maxDashCounts;
    [SerializeField] float gravity = -9.81f;

    [Header("Mech Stat")]
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    [SerializeField] float detectionRange;

    [Header("Damage/Bullet Stat")]
    public GameObject basicBullet;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletTurnSpeed;
    public Transform firePoint;

    [SerializeField]Transform target;
    [SerializeField]Transform meshRot;

    CharacterController controller;
    Vector3 playerVelocity;
    float dashTimer = 0f;
    Vector3 dashDirection = Vector3.zero;
    bool isDashing = false;
    Vector3 lastFacingDirection = Vector3.forward;
    [SerializeField] float rotationLerpSpeed = 5f;

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
            ShootControl(Input.GetKeyDown(KeyCode.F));
        }
        else
        {
            MoveControl(Input.GetAxis("HorizontalPlayer2"), Input.GetAxis("VerticalPlayer2"),Input.GetKeyDown(KeyCode.Keypad2));
            ShootControl(Input.GetKeyDown(KeyCode.Keypad1));
        }
        targetDetection();
        
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

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void targetDetection(){
        if(!isPlayer2)
        {
           Collider[] hit = Physics.OverlapSphere(transform.position,detectionRange,LayerMask.GetMask("Player2"));
            if(hit.Length > 0){
                Debug.Log("Detected 2");
                target = hit[0].transform;
            }
            else
            {
                target = null;
            }
        }
        else{
           Collider[] hit = Physics.OverlapSphere(transform.position,detectionRange,LayerMask.GetMask("Player1"));
            if(hit.Length > 0){
                Debug.Log("Detected 1");
                target = hit[0].transform;
            }
            else
            {
             target = null;
            }
        }
        
    }

    public void ShootControl(bool shootInp)
    {
        if (shootInp)
        {
            GameObject basicbullet = Instantiate(basicBullet, firePoint.position, firePoint.rotation);
            Bullet bulletScript = basicbullet.GetComponent<Bullet>();
            if(target != null){
                bulletScript.SetTarget(target, bulletDamage, bulletSpeed, bulletTurnSpeed);
            }
            else{
                bulletScript.SetTarget(null, bulletDamage, bulletSpeed, bulletTurnSpeed);
                basicBullet.transform.forward = lastFacingDirection;
            }
            
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
        
        if(target != null) 
        {
            lastFacingDirection = (target.position - transform.position).normalized;
        }
        else if(move != Vector3.zero && target == null)
        {
            lastFacingDirection = move;
        }
        
        meshRot.rotation = Quaternion.Lerp(meshRot.rotation, Quaternion.LookRotation(lastFacingDirection), Time.deltaTime * rotationLerpSpeed);//rotate mesh to move direction with lerp
        
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}
