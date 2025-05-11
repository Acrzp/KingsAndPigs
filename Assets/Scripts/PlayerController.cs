using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidbody2D;
    private GatherInput _gatherInput;
    [SerializeField] private Transform myTransform;
    private Animator _animator;

    //Animator Ids
    private readonly int _idSpeed = Animator.StringToHash("speed");
    private readonly int _idIsGrounded = Animator.StringToHash("isGrounded");
    private readonly int _idIsWallDetected = Animator.StringToHash("isWallDetected");
    private readonly int _idKnockback = Animator.StringToHash("knockback");
    private readonly int _idIdle = Animator.StringToHash("Idle");
    private readonly int _idDoorIn = Animator.StringToHash("doorIn");

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

    private void Awake() //Obtengo los componentes a los que esta asociado el GameObject
    {
        _gatherInput = GetComponent<GatherInput>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        CheckPlayerRespawnState();
    }

    private void Start() => counterExtraJumps = extraJumps;

    private void CheckPlayerRespawnState()
    {
        if (GameManager.Instance.hasCheckpointActive)
        {
            canMove = true;
            StartInCheckPoint();
            
        }
        else
        {
            canMove = false;
            StartCoroutine(CanMoveRoutine());
        }
    }

    private void StartInCheckPoint() => _animator.Play(_idIdle);

    private void Update()  //Buscar mantener el Update con pocas instrucciones
    {
        SetAnimatorValues();
    }

    private void SetAnimatorValues()
    {
        _animator.SetFloat(_idSpeed, Mathf.Abs(_rigidbody2D.linearVelocityX));
        _animator.SetBool(_idIsGrounded, isGrounded);
        _animator.SetBool(_idIsWallDetected, isWallDetected);
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
        else isGrounded = false;
    }

    private void HandleWalls()
    {
        isWallDetected = Physics2D.Raycast(myTransform.position, Vector2.right * _direction, checkWallDistance, groundLayer);
    }

    private void HandleWallSlide()
    {
        canWallSlide = isWallDetected;
        if (!canWallSlide) return;
        canExtraJump = false;
        slideSpeed =_gatherInput.Value.y < 0 ? 1 : 0.5f; //Si el valor es menor a 0 (y=-1), slideSpeed es igual a 1, sino a 0.5
        _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocityX,_rigidbody2D.linearVelocityY * slideSpeed);
    }

    private void Move()
    {
        if (!canMove) return;
        if (isWallDetected && !isGrounded) return;
        if (isWallJumping) return;
        Flip();
        _rigidbody2D.linearVelocity = new Vector2(speed * _gatherInput.Value.x, _rigidbody2D.linearVelocityY);
    }
    
    private IEnumerator CanMoveRoutine()
    {
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }

    private void Flip()
    {
        if(_gatherInput.Value.x * _direction < 0) HandleDirection();
    }

    private void HandleDirection()
    {
        myTransform.localScale = new Vector3(-myTransform.localScale.x, myTransform.localScale.y, myTransform.localScale.z);
        _direction *= -1;
    }

    private void Jump()
    {
        if(_gatherInput.IsJumping)
        {
            if(isGrounded)
            {
                _rigidbody2D.linearVelocity = new Vector2(speed * _gatherInput.Value.x, jumpForce);  
                canExtraJump = true;
            }
            else if (isWallDetected) WallJump();
            else if (counterExtraJumps > 0 && canExtraJump) ExtraJump();
        }
        _gatherInput.IsJumping = false;
    }

    private void ExtraJump()
    {
        _rigidbody2D.linearVelocity = new Vector2(speed * _gatherInput.Value.x, jumpForce);
        counterExtraJumps--;
    }
    
    private void WallJump()
    {
        _rigidbody2D.linearVelocity = new Vector2 (wallJumpForce.x * -_direction, wallJumpForce.y); 
        HandleDirection();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine() //Corutina (Se ejecuta en 2do plano sin afectar el juego)
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }

    public void Knockback()
    {
        StartCoroutine(KnockbackRoutine());
        _rigidbody2D.linearVelocity = new Vector2 (knockPower.x * -_direction, knockPower.y);
        _animator.SetTrigger(_idKnockback);
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
        GameObject _deathVFXPrefab = Instantiate(deathVFX, myTransform.position, Quaternion.identity); 
        //Instanciar desde un prefab al deathVFX en la posicion y orientacion del player
        Destroy(gameObject);
    }

    public void DoorIn()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _animator.Play(_idIdle); //Se forza el Idle después de detener el movimiento
        _animator.SetBool(_idDoorIn, true);
        canMove = false;
        StartCoroutine(DoorInRoutine());
    }

    private IEnumerator DoorInRoutine()
    {
        yield return new WaitForSeconds(moveDelay);
        SceneManager.LoadScene(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(myTransform.position, new Vector2(myTransform.position.x + (checkWallDistance * _direction), myTransform.position.y));
    }

}
