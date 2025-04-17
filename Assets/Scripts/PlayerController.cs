using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Componentes
    private Rigidbody2D m_rigidbody2D;
    private GatterInput m_gatherInput;
    private Transform m_transform;
    private Animator m_animator;

    //Valores
    [SerializeField] private float speed;
    private int direction = 1;
    private int idSpeed;

    
    void Start() //Obtengo los componentes a los que esta asociado el GameObject
    {
        m_gatherInput = GetComponent<GatterInput>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_transform = GetComponent<Transform>();
        m_animator = GetComponent<Animator>();
        idSpeed = Animator.StringToHash("Speed");
    }

    void Update()  //Buscar mantener el Update con pocas instrucciones
    {
        SetAnimatorValues();
    }

    private void SetAnimatorValues()
    {
        m_animator.SetFloat(idSpeed, Mathf.Abs(m_rigidbody2D.linearVelocityX));
    }

    void FixedUpdate()
    {
        Move();
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
}
