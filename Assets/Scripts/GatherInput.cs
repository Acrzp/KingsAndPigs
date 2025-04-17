using UnityEngine;
using UnityEngine.InputSystem;


public class GatterInput : MonoBehaviour
{
    private Controls controls;
    [SerializeField] private float _valueX; //Serialize me permite visualizar y modificar en el inspector un valor

    public float ValueX { get => _valueX; } //Enapsulamiento para acceder a la variable

    private void Awake() //Primer método que se manda a llamar 
    {
        controls = new Controls();

    }

    private void OnEnable()
    {
        controls.Player.Move.performed += StartMove; //Sintaxis para asociar un evento con un mñetodo
        controls.Player.Move.canceled += StopMove; //Revisar documentación para ver los tipos de interaccciones con las entradas
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


    private void OnDisable()
    {
        controls.Player.Move.performed -= StartMove;
        controls.Player.Move.canceled -= StopMove;
        controls.Player.Disable();
    }
}
