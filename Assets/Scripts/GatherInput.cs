using UnityEngine;
using UnityEngine.InputSystem;


public class GatterInput : MonoBehaviour
{
    //Componentes
    private Controls controls;


    //Valores
    [SerializeField] private float _valueX; //Serialize me permite visualizar y modificar en el inspector un valor
    public float ValueX { get => _valueX; } //Enapsulamiento para acceder a la variable

    [SerializeField] private bool _isJumping;
    public bool IsJumping { get => _isJumping; set => _isJumping = value; }
    

    private void Awake() //Primer método que se manda a llamar 
    {
        controls = new Controls();

    }

    private void OnEnable()
    {
        controls.Player.Move.performed += StartMove; //Sintaxis para asociar un evento con un mñetodo
        controls.Player.Move.canceled += StopMove; //Revisar documentación para ver los tipos de interaccciones con las entradas
        controls.Player.Jump.performed += StartJump;
        controls.Player.Jump.canceled += StopJump;
        controls.Player.Enable();
    }

    private void StartMove(InputAction.CallbackContext context) 
    {
        _valueX = context.ReadValue<float>();
    }

    private void StopMove(InputAction.CallbackContext context)
    {
        _valueX = 0;
    }

    private void StartJump(InputAction.CallbackContext context)
    {
        _isJumping = true;
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        _isJumping = false; 
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= StartMove;
        controls.Player.Move.canceled -= StopMove;
        controls.Player.Jump.performed -= StartJump;
        controls.Player.Jump.canceled -= StopJump;
        controls.Player.Disable();
    }
}
