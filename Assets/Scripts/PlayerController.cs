using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D m_rigidbody2D;
    private GatterInput m_gatherInput;
    [SerializeField] private Transform m_transform;
    private Animator m_animator;

    //Animator Ids
    private int idSpeed;
    private int idIsGrounded;
    private int idIsWallDetected;


    [Header("Move settings")]
    [SerializeField] private float speed;
    private int direction = 1;
    
    [Header("Jump settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int extraJumps;
    [SerializeField] private float counterExtraJumps;
    [SerializeField] private bool canExtraJump;

    [Header("Ground settings")]
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;
    RaycastHit2D lFootRay;
    RaycastHit2D rFootRay;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float rayLength;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall settings")]
    [SerializeField] private float checkWallDistance;
    [SerializeField] private bool isWallDetected;
    [SerializeField] private bool canWallSlide;
    [SerializeField] private float slideSpeed;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpDuration;

    private void Awake()
    {
        m_gatherInput = GetComponent<GatterInput>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        //m_transform = GetComponent<Transform>();
        m_animator = GetComponent<Animator>();
    }

    void Start() //Obtengo los componentes a los que esta asociado el GameObject
    {
        idSpeed = Animator.StringToHash("speed");
        idIsGrounded = Animator.StringToHash("isGrounded");
        idIsWallDetected = Animator.StringToHash("isWallDetected");
        lFoot = GameObject.Find("LFoot").GetComponent<Transform>();
        rFoot = GameObject.Find("RFoot").GetComponent<Transform>();
        counterExtraJumps = extraJumps;
    }

    void Update()  //Buscar mantener el Update con pocas instrucciones
    {
        SetAnimatorValues();
    }

    private void SetAnimatorValues()
    {
        m_animator.SetFloat(idSpeed, Mathf.Abs(m_rigidbody2D.linearVelocityX));
        m_animator.SetBool(idIsGrounded, isGrounded);
        m_animator.SetBool(idIsWallDetected, isWallDetected);
    }

    void FixedUpdate()
    {
        CheckCollision();
        Move();
        Jump();
    }

    private void CheckCollision()
    {
        HandleGround();
        HandleWalls();
        HandleWallSlide();
    }

    private void HandleGround()
    {
         lFootRay = Physics2D.Raycast(lFoot.position, Vector2.down, rayLength, groundLayer);
         rFootRay = Physics2D.Raycast(rFoot.position, Vector2.down, rayLength, groundLayer);

        if (lFootRay || rFootRay)
        {
            isGrounded = true;
            counterExtraJumps = extraJumps;
            canExtraJump = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleWalls()
    {
        isWallDetected = Physics2D.Raycast(m_transform.position, Vector2.right * direction, checkWallDistance, groundLayer);
    }

    private void HandleWallSlide()
    {
        canWallSlide = isWallDetected;
        if (!canWallSlide) return;
        canExtraJump = false;
        slideSpeed =m_gatherInput.Value.y < 0 ? 1 : 0.5f; //Si el valor es menor a 0 (y=-1), slideSpeed es igual a 1, sino a 0.5
        m_rigidbody2D.linearVelocity = new Vector2(m_rigidbody2D.linearVelocityX,m_rigidbody2D.linearVelocityY * slideSpeed);
    }

    private void Move()
    {
        if (isWallDetected && !isGrounded) return;
        if (isWallJumping) return;
        Flip();
        m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.Value.x, m_rigidbody2D.linearVelocityY);
    }

    private void Flip()
    {
        if(m_gatherInput.Value.x * direction < 0)
        {
            HandleDirection();
        }
    }

    private void HandleDirection()
    {
        m_transform.localScale = new Vector3(-m_transform.localScale.x, m_transform.localScale.y, m_transform.localScale.z);
        direction *= -1;
    }

    private void Jump()
    {
        if(m_gatherInput.IsJumping)
        {
            if(isGrounded)
            {
                m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.Value.x, jumpForce);  
                canExtraJump = true;
            }
            else if (isWallDetected) WallJump();
            else if (counterExtraJumps > 0 && canExtraJump) ExtraJump();
        }
        m_gatherInput.IsJumping = false;
    }


    private void ExtraJump()
    {
        m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.Value.x, jumpForce);
        counterExtraJumps--;
    }
    
    private void WallJump()
    {
        m_rigidbody2D.linearVelocity = new Vector2 (wallJumpForce.x * -direction, wallJumpForce.y); 
        HandleDirection();
        StartCoroutine(WallJumpRoutine());
    }

    IEnumerator WallJumpRoutine() //Corutina (Se ejecuta en 2do plano sin afectar el juego)
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping =false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_transform.position, new Vector2(m_transform.position.x + (checkWallDistance * direction), m_transform.position.y));
    }

}
