using UnityEngine;
using UnityEngine.InputSystem;


public class GatterInput : MonoBehaviour
{
    //Componentes
    private Controls _controls;

    //Valores
    [SerializeField] private Vector2 value; //Serialize me permite visualizar y modificar en el inspector un valor
    public Vector2 Value => value;  //Enapsulamiento para acceder a la variable

    [SerializeField] private bool isJumping;
    public bool IsJumping { get => isJumping; set => isJumping = value; }
    
    private void Awake() //Primer m�todo que se manda a llamar 
    {
        _controls = new Controls();

    }

    private void OnEnable()
    {
        _controls.Player.Move.performed += StartMove; //Sintaxis para asociar un evento con un metodo
        _controls.Player.Move.canceled += StopMove; //Revisar documentacion para ver los tipos de interaccciones con las entradas
        _controls.Player.Jump.performed += StartJump;
        _controls.Player.Jump.canceled += StopJump;
        _controls.Player.Enable();
    }

    private void StartMove(InputAction.CallbackContext context) 
    {
        value = context.ReadValue<Vector2>().normalized;
        
    }

    private void StopMove(InputAction.CallbackContext context)
    {
        value = Vector2.zero;
    }

    private void StartJump(InputAction.CallbackContext context)
    {
        isJumping = true;
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        isJumping = false; 
    }

    private void OnDisable()
    {
        _controls.Player.Move.performed -= StartMove;
        _controls.Player.Move.canceled -= StopMove;
        _controls.Player.Jump.performed -= StartJump;
        _controls.Player.Jump.canceled -= StopJump;
        _controls.Player.Disable();
    }
}
