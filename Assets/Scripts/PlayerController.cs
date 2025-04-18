using System;
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

    }

    private void HandleWalls()
    {
        isWallDetected = Physics2D.Raycast(m_transform.position, Vector2.right * direction, checkWallDistance, groundLayer);
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

    private void Move()
    {
        Flip();
        m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.ValueX, m_rigidbody2D.linearVelocityY);
    }

    private void Flip()
    {
        if(m_gatherInput.ValueX * direction < 0)
        {
            m_transform.localScale = new Vector3(-m_transform.localScale.x, m_transform.localScale.y, m_transform.localScale.z);
            direction *= -1;
        }
    }

    private void Jump()
    {
        if(m_gatherInput.IsJumping)
        {
            if(isGrounded)
            {
                m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.ValueX, jumpForce);  
                canExtraJump = true;
            }
            else if (counterExtraJumps > 0 && canExtraJump)
            {
                m_rigidbody2D.linearVelocity = new Vector2(speed * m_gatherInput.ValueX, jumpForce);
                counterExtraJumps--;
            }
        }
        m_gatherInput.IsJumping = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_transform.position, new Vector2(m_transform.position.x + (checkWallDistance * direction), m_transform.position.y));
    }

}
