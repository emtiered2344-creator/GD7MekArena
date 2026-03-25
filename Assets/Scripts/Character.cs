using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Character : MonoBehaviour, IMovement
{
    [SerializeField] float speed;
    [SerializeField] float dashSpeed;
    [SerializeField] float gravity = -9.81f;

    public bool isPlayer2;
    CharacterController controller;
    Vector3 playerVelocity;
    InputActionReference moveAction;

    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveControl();
    }
    void OnEnable()
    {
        
    }
    public void Move(Vector3 direction)
    {
        controller.Move(direction * speed * Time.deltaTime);
    }
    public void MoveControl()
    {
        if(controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;//reset gravity when grounded
        }

        playerVelocity.y += gravity * Time.deltaTime;//gravity
        Vector3 move = Vector3.zero;//intialize move
        if(!isPlayer2)
        {
            //Player 1 movement (wasd)
            float xInp = Input.GetAxis("HorizontalPlayer1");
            float zInp = Input.GetAxis("VerticalPlayer1");
            move = transform.right * xInp + transform.forward * zInp;
        }
        else
        {
            //Player 2 movement (arrowkeys)
            float xInp = Input.GetAxis("HorizontalPlayer2");
            float zInp = Input.GetAxis("VerticalPlayer2");
            move = transform.right * xInp + transform.forward * zInp;
        }
        Vector3 finalMove = move + Vector3.up * playerVelocity.y;//final move
        Move(finalMove);
    }
}
