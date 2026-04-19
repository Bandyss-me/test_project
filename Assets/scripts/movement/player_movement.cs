using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using TMPro;

public class player_movement : MonoBehaviour
{
    CharacterController playerController;

    [Header("Movement")]
    [SerializeField]
    float walk_speed=5f;
    [SerializeField]
    float run_speed=10f;
    [SerializeField]
    float air_speed=2f;

    [Header("Jumping")]
    [SerializeField]
    float jumpForce=4f;
    [SerializeField]
    float jumpTimeLimit=0.4f;

    [Header("Camera")]
    [SerializeField]
    Transform playerCamera;
    [SerializeField]
    float mouseSensitivity=3f;

    //other
    bool _hasGravity=true;
    bool _isGrounded;
    bool _isInAir;

    //jumping
    bool _hasLanded;
    bool _hasJumped;
    float _heigthTimer;

    float verMov;
    float _cameraPitch;
    Vector3 _movementV;
    Vector3 _prevMovementV;

    void Awake(){
        playerController=GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _hasLanded = false;
        _hasJumped = false;
    }

    void Update(){
        GroundCheck();
        Move();
        RotateWithMouse();
    }

    void GroundCheck(){
        _isGrounded = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hit, 0.75f,1,QueryTriggerInteraction.Ignore);
        _isInAir = !Physics.Raycast(transform.position-transform.up*1f,Vector3.down,1f);
    }

    void Newton_is_watching_you(){
        if(_hasGravity){
            if (!_isGrounded){
                verMov-=9.8f*Time.deltaTime;
            }
            else{
                verMov=0;
            }
        }
    }

    void Move(){
        Vector3 input=new Vector3(Input.GetAxis("Horizontal"),0f,Input.GetAxis("Vertical"));
        Vector3 horinput=transform.TransformDirection(input);
        float tspeed=(Input.GetKey(KeyCode.LeftShift))?run_speed:walk_speed;
        _movementV.x=horinput.x*tspeed;
        _movementV.z=horinput.z*tspeed;
        Jump();
        Newton_is_watching_you();
        _movementV.y+=verMov;
        _movementV*=Time.deltaTime;
        if(_isInAir){
            _movementV.x=_prevMovementV.x;
            _movementV.z=_prevMovementV.z;
        }
        playerController.Move(_movementV);
        _prevMovementV=_movementV;
    }

    void RotateWithMouse(){
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
        playerCamera.transform.localEulerAngles = new Vector3(_cameraPitch, 0f, 0f);
    }

    void Jump(){
        if (Input.GetKey(KeyCode.Space) && _hasLanded)
        {
            verMov=0f;
            verMov+=jumpForce;
            _heigthTimer=0f;
            _hasJumped=true;
            _hasGravity=false;
            _hasLanded=false;
        }
        if (Input.GetKey(KeyCode.Space) && _heigthTimer<jumpTimeLimit)
        {
            _heigthTimer+=Time.deltaTime;
            verMov-=Time.deltaTime;
        }
        else if (_hasJumped)
        {
            _hasJumped=false;
            if(!_hasGravity)
                _hasGravity=true;
        }
        if(_isGrounded && !_hasLanded)
        {
            _hasLanded=true;
        }
    }
}