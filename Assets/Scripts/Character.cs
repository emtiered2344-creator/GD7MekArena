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
    public float magSize;
    public float fireRate;
    public float reloadTime;

    [SerializeField]Transform target;
    [SerializeField]Transform meshRot;

    CharacterController controller;
    Vector3 playerVelocity;
    float dashTimer = 0f;
    Vector3 dashDirection = Vector3.zero;
    public bool isDashing = false;
    Vector3 lastFacingDirection = Vector3.forward;
    [SerializeField] float rotationLerpSpeed = 5f;
    
    float fireRateTimer = 0f;
    float currentMagSize;
    float currentReloadTime;

    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        dashCounts = maxDashCounts;
        bulletTurnSpeed = 7.5f / bulletSpeed;
        currentMagSize = magSize;
        currentReloadTime = reloadTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlayer2)//player1
        {
            MoveControl(Input.GetAxis("HorizontalPlayer1"), Input.GetAxis("VerticalPlayer1"),Input.GetKeyDown(KeyCode.G));
            if(Input.GetKey(KeyCode.F))
            {
                if(fireRateTimer >= fireRate && currentMagSize > 0)
                shootBullet();
            }
        }
        else//player2
        {
            MoveControl(Input.GetAxis("HorizontalPlayer2"), Input.GetAxis("VerticalPlayer2"),Input.GetKeyDown(KeyCode.Keypad2));
            if(Input.GetKey(KeyCode.Keypad1))
            {
                if(fireRateTimer >= fireRate && currentMagSize > 0)
                shootBullet();
            }
        }
        targetDetection();
        
        // Update fire rate timer
        if(fireRateTimer <= fireRate)//firerate boom boom
        {
            fireRateTimer += Time.deltaTime;
        }

        if(currentMagSize <= 0)//reloading weapon
        {
            currentReloadTime -= Time.deltaTime;
            if(currentReloadTime <= 0)
            {
                currentMagSize = magSize;
                currentReloadTime = reloadTime;
            }
            
        }
    }
    public void Move(Vector3 direction)//movement method
    {
        controller.Move(direction * speed * Time.deltaTime);
    }
    public void Dash(Vector3 direction)//dash method
    {
        if (!isDashing)
        {
            isDashing = true;
            dashTimer = 0f;
            dashDirection = direction.normalized;
        }
    }

    public void TakeDamage(float damage)//hit method
    {
        health -= damage;
    }

    public void targetDetection(){//target detection method
        if(!isPlayer2)//player1
        {
            Collider[] hit = Physics.OverlapSphere(transform.position,detectionRange,LayerMask.GetMask("Player2"));
            if(hit.Length > 0){
                target = hit[0].transform;

                Ray ray = new Ray(transform.position, target.position - transform.position);
                if(Physics.Raycast(ray, out RaycastHit hitInfo, detectionRange, LayerMask.GetMask("Obstacles")))
                {
                    Debug.Log("Target Lost");
                    target = null;
                }

            }
            else
            {
                target = null;
            }
        }
        else{//player2
           Collider[] hit = Physics.OverlapSphere(transform.position,detectionRange,LayerMask.GetMask("Player1"));
            if(hit.Length > 0){
                target = hit[0].transform;

                Ray ray = new Ray(transform.position, target.position - transform.position);
                if(Physics.Raycast(ray, out RaycastHit hitInfo, detectionRange, LayerMask.GetMask("Obstacles")))
                {
                    Debug.Log("Target Lost");
                    target = null;
                }

            }
            else
            {
             target = null;
            }
        }
        
    }
    public void shootBullet()
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
        currentMagSize--;
        fireRateTimer = 0f;
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
