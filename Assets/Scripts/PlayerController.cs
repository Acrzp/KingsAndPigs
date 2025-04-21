using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _mRigidbody2D;
    private GatterInput _mGatherInput;
    [SerializeField] private Transform mTransform;
    private Animator _mAnimator;

    //Animator Ids
    private int _idSpeed;
    private int _idIsGrounded;
    private int _idIsWallDetected;
    private int _idKnockback;

    [Header("Move settings")]
    [SerializeField] private float speed;
    [SerializeField] private bool canMove;
    [SerializeField] private float moveDelay;
    private int _direction = 1;
    
    [Header("Jump settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int extraJumps;
    [SerializeField] private float counterExtraJumps;
    [SerializeField] private bool canExtraJump;

    [Header("Ground settings")]
    [SerializeField] private Transform lFoot;
    [SerializeField] private Transform rFoot;
    private RaycastHit2D _lFootRay;
    private RaycastHit2D _rFootRay;
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

    [Header("Knocked settings")]
    [SerializeField] private bool isKnocked;
    //[SerializeField] private bool canBeKnocked;
    [SerializeField] private Vector2 knockPower;
    [SerializeField] private float knockedDuration;

    [Header("Death VFX")]
    [SerializeField] private GameObject deathVFX;

    private void Awake()
    {
        _mGatherInput = GetComponent<GatterInput>();
        _mRigidbody2D = GetComponent<Rigidbody2D>();
        //m_transform = GetComponent<Transform>();
        _mAnimator = GetComponent<Animator>();
        canMove = false;
        StartCoroutine(CanMoveRoutine());
    }

    private void Start() //Obtengo los componentes a los que esta asociado el GameObject
    {
        _idSpeed = Animator.StringToHash("speed");
        _idIsGrounded = Animator.StringToHash("isGrounded");
        _idIsWallDetected = Animator.StringToHash("isWallDetected");
        _idKnockback = Animator.StringToHash("knockback");
        counterExtraJumps = extraJumps;
    }

    private void Update()  //Buscar mantener el Update con pocas instrucciones
    {
        SetAnimatorValues();
    }

    private void SetAnimatorValues()
    {
        _mAnimator.SetFloat(_idSpeed, Mathf.Abs(_mRigidbody2D.linearVelocityX));
        _mAnimator.SetBool(_idIsGrounded, isGrounded);
        _mAnimator.SetBool(_idIsWallDetected, isWallDetected);
    }

    private void FixedUpdate()
    {
        if(!canMove) return;
        if(isKnocked) return;
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
         _lFootRay = Physics2D.Raycast(lFoot.position, Vector2.down, rayLength, groundLayer);
         _rFootRay = Physics2D.Raycast(rFoot.position, Vector2.down, rayLength, groundLayer);

        if (_lFootRay || _rFootRay)
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
        isWallDetected = Physics2D.Raycast(mTransform.position, Vector2.right * _direction, checkWallDistance, groundLayer);
    }

    private void HandleWallSlide()
    {
        canWallSlide = isWallDetected;
        if (!canWallSlide) return;
        canExtraJump = false;
        slideSpeed =_mGatherInput.Value.y < 0 ? 1 : 0.5f; //Si el valor es menor a 0 (y=-1), slideSpeed es igual a 1, sino a 0.5
        _mRigidbody2D.linearVelocity = new Vector2(_mRigidbody2D.linearVelocityX,_mRigidbody2D.linearVelocityY * slideSpeed);
    }

    private void Move()
    {
        if (!canMove) return;
        if (isWallDetected && !isGrounded) return;
        if (isWallJumping) return;
        Flip();
        _mRigidbody2D.linearVelocity = new Vector2(speed * _mGatherInput.Value.x, _mRigidbody2D.linearVelocityY);
    }
    
    private IEnumerator CanMoveRoutine()
    {
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }

    private void Flip()
    {
        if(_mGatherInput.Value.x * _direction < 0)
        {
            HandleDirection();
        }
    }

    private void HandleDirection()
    {
        mTransform.localScale = new Vector3(-mTransform.localScale.x, mTransform.localScale.y, mTransform.localScale.z);
        _direction *= -1;
    }

    private void Jump()
    {
        if(_mGatherInput.IsJumping)
        {
            if(isGrounded)
            {
                _mRigidbody2D.linearVelocity = new Vector2(speed * _mGatherInput.Value.x, jumpForce);  
                canExtraJump = true;
            }
            else if (isWallDetected) WallJump();
            else if (counterExtraJumps > 0 && canExtraJump) ExtraJump();
        }
        _mGatherInput.IsJumping = false;
    }

    private void ExtraJump()
    {
        _mRigidbody2D.linearVelocity = new Vector2(speed * _mGatherInput.Value.x, jumpForce);
        counterExtraJumps--;
    }
    
    private void WallJump()
    {
        _mRigidbody2D.linearVelocity = new Vector2 (wallJumpForce.x * -_direction, wallJumpForce.y); 
        HandleDirection();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine() //Corutina (Se ejecuta en 2do plano sin afectar el juego)
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping =false;
    }

    public void Knockback()
    {
        StartCoroutine(KnockbackRoutine());
        _mRigidbody2D.linearVelocity = new Vector2 (knockPower.x * -_direction, knockPower.y);
        _mAnimator.SetTrigger(_idKnockback);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        //canBeKnocked = false;
        yield return new WaitForSeconds(knockedDuration);
        isKnocked = false;
        //canBeKnocked = true;
    }

    public void Die()
    {
        GameObject _deathVFXPrefab = Instantiate(deathVFX, mTransform.position, Quaternion.identity); 
        //Instanciar desde un prefab al deathVFX en la posicion y orientacion del player
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mTransform.position, new Vector2(mTransform.position.x + (checkWallDistance * _direction), mTransform.position.y));
    }

}
