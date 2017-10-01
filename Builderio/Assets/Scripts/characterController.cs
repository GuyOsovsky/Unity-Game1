using UnityEngine;
using System.Collections;

public class characterController : MonoBehaviour
{

    private float ForwardInput, RightInput, CameraUpInput, CameraRightInput, JumpInput;
    private CharacterController CharacterController;
    private Vector3 Velocity;
    private float DelayInput = 0.1f;
    private float ForwardVel = 6;
    private float TurnVel = 10;
    private Quaternion TargetRotationX;
    private Quaternion TargetRotationY;
    private float DistToGround = 1.1f;
    public LayerMask Ground;
    private float MinimumX = -90f;
    private float MaximumX = 90f;
    private bool LockCursorState;

    // Use this for initialization
    void Start()
    {
        CharacterController = gameObject.GetComponent<CharacterController>();
        TargetRotationX = gameObject.transform.rotation;
        TargetRotationY = gameObject.transform.rotation;
        Velocity = Vector3.zero;
        LockCursorState = true;
    }
    void CursorUpdate()
    {
        LockCursorState = !gameObject.GetComponent<GameLogic>().GetCursorLock();
        if (LockCursorState)
        {
            LockCursor();
            return;
        }
        UnLockCursor();
    }
    void GetInput()
    {
        ForwardInput = Input.GetAxis("Vertical");
        RightInput = Input.GetAxis("Horizontal");
        CameraRightInput = Input.GetAxis("Mouse Y");
        CameraUpInput = Input.GetAxis("Mouse X");
        JumpInput = Input.GetAxisRaw("Jump");
    }
    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<ChunksController>().GetInGame())
        {
            Velocity = new Vector3(0, Velocity.y, 0);
            CursorUpdate();
            GetInput();
            if (LockCursorState)
            {
                Run();
                turn();
            }
            Jump();
            CharacterController.Move(Velocity * Time.deltaTime);
        }
    }
    void Run()
    {
        if (Mathf.Abs(ForwardInput) > DelayInput)
        {
            //if(ForwardInput > 0)
            //    if (!Physics.Raycast(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.GetComponent<CapsuleCollider>().height / 2, gameObject.transform.position.z), gameObject.transform.forward, gameObject.GetComponent<CapsuleCollider>().radius / 2, Ground))
            //        if (!Physics.Raycast(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.GetComponent<CapsuleCollider>().height / 2, gameObject.transform.position.z), gameObject.transform.forward, gameObject.GetComponent<CapsuleCollider>().radius / 2, Ground))
            //            Rbody.velocity += gameObject.transform.forward * ForwardInput * ForwardVel;
            //if (ForwardInput < 0)
            //    if (!Physics.Raycast(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + gameObject.GetComponent<CapsuleCollider>().height / 2, gameObject.transform.position.z), -gameObject.transform.forward, gameObject.GetComponent<CapsuleCollider>().radius / 2, Ground))
            //        if (!Physics.Raycast(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - gameObject.GetComponent<CapsuleCollider>().height / 2, gameObject.transform.position.z), -gameObject.transform.forward, gameObject.GetComponent<CapsuleCollider>().radius / 2, Ground))
            Velocity += gameObject.transform.forward * ForwardInput * ForwardVel;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Velocity += gameObject.transform.forward * ForwardInput * ForwardVel;
            }
        }

        if (Mathf.Abs(RightInput) > DelayInput)
        {
            Velocity += gameObject.transform.right * RightInput * ForwardVel;
        }

    }
    void Jump()
    {
        if(JumpInput == 1 && Grounded())
        {
            //Rbody.AddForce(Vector3.up * 200f, ForceMode.Acceleration);
            Velocity = new Vector3(Velocity.x, 7, Velocity.z);
        }
        else if (JumpInput == 0 && Grounded())
        {
            Velocity = new Vector3(Velocity.x, 0, Velocity.z);
        }
        else
        {
            Velocity = new Vector3(Velocity.x, Velocity.y - (9.8f * Time.deltaTime), Velocity.z);
        }
    }
    bool Grounded()
    {
        //return Physics.Raycast(gameObject.transform.position, Vector3.down, DistToGround, Ground);
        return CharacterController.isGrounded;
    }
    void turn()
    {
        TargetRotationY *= Quaternion.AngleAxis(CameraUpInput * TurnVel, gameObject.transform.up);
        gameObject.transform.localRotation = TargetRotationY;
        TargetRotationX *= Quaternion.AngleAxis(-CameraRightInput * TurnVel, Vector3.right);
        TargetRotationX = ClampRotationAroundXAxis(TargetRotationX);
        gameObject.transform.GetChild(0).gameObject.transform.localRotation = TargetRotationX;
    }
    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
