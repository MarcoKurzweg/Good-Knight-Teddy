using UnityEngine;
using System.Collections;

public class Character_Controller : MonoBehaviour
{
    [System.Serializable]
    public class MoveSettings
    {
        public float forwardVel = 12;
        public float rotateVel = 100;
        public float jumpVel = 25;
        public float distToGrounded = 0.5f;
        public LayerMask ground;
    }

    [System.Serializable]
    public class PhysSettings
    {
        public float downAccel = 0.75f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
    }

    public MoveSettings moveSettings = new MoveSettings();
    public PhysSettings physSettings = new PhysSettings();
    public InputSettings inputSettings = new InputSettings();

    Vector3 velocity = Vector3.zero;
    Quaternion targetRotation;
    Rigidbody rigidBody;
    float forwardInput, turnInput, jumpInput;

    public Animator anim;

    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    bool Grounded() //boolean to check if player is grounded
    {
        return Physics.Raycast(transform.position, Vector3.down, moveSettings.distToGrounded, moveSettings.ground);
    }

    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
            rigidBody = GetComponent<Rigidbody>();
        else
            Debug.LogError("Character needs a rigidbody");

        forwardInput = turnInput = jumpInput = 0;

        anim = GetComponent<Animator>();
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS);
        turnInput = Input.GetAxis(inputSettings.TURN_AXIS);
        jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS);
    }

    void Update()
    {
        GetInput();
        
        if(forwardInput == 0) //if not moving, defautls to idle animation
        {
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsWalking", false);
        }
    }

    void FixedUpdate() //handles all character movement
    {
        Run();
        Jump();
        Turn();

        rigidBody.velocity = transform.TransformDirection(velocity);
    }

    void Run()
    {
        if(Mathf.Abs(forwardInput) > inputSettings.inputDelay && Input.GetKey(KeyCode.LeftShift))
        {
            //walk
            velocity.z = moveSettings.forwardVel * forwardInput * 0.25f;
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsWalking", true);
        }
        else if (Mathf.Abs(forwardInput) > inputSettings.inputDelay)
        {
            //run
            velocity.z = moveSettings.forwardVel * forwardInput;
            anim.SetBool("IsRunning", true);
            anim.SetBool("IsWalking", false);
        }
        else
            //zero velocity
            velocity.z = 0;
    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) > inputSettings.inputDelay) //Controls the rotation of the character
        {
            targetRotation *= Quaternion.AngleAxis(moveSettings.rotateVel * turnInput * Time.deltaTime, Vector3.up);
        }
        transform.rotation = targetRotation;

        if(Input.GetKey(KeyCode.Q)) //Strafes Left
        {
            transform.Translate(-0.01f,0,0 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E)) //Strafes Right
        {
            transform.Translate(0.01f, 0, 0 * Time.deltaTime);
        }
    }

    void Jump()
    {
        if(jumpInput > 0 && Grounded()) //Checks if character is grounded, if he is, he can jump
        {
            //jump
            velocity.y = moveSettings.jumpVel;

            anim.Play("Jump");
        }
        else if (jumpInput == 0 && Grounded())
        {
            //zero out velocity.y
            velocity.y = 0;
        }
        else
        {
            //decrease velocity.y
            velocity.y -= physSettings.downAccel;
        }
    }
}
