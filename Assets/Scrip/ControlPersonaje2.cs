using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPersonaje2 : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Animator animator;

    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminata = 2f;
    [SerializeField] private float velocidadCarrera = 5f;
    [SerializeField] private float suavizadoDeMovimiento = 0.1f;

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto = 5f;
    [SerializeField] private Transform sueloCheck;
    [SerializeField] private LayerMask sueloLayer;
    [SerializeField] private float gravedadAdicional = 5f;
    private bool estaEnElSuelo = true;

    [Header("Cámara")]
    [SerializeField] private float sensibilidadMouse = 100f;
    [SerializeField] private Transform camara;

    private Vector3 velocidad = Vector3.zero;
    private float velocidadActual = 0f;
    private float rotacionX = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        Saltar();
        ControlarCamara();
    }

    private void Movimiento()
    {
        // Obtener entrada del jugador
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");
        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift);

        // Calcular velocidad deseada
        Vector3 direccion = new Vector3(movimientoHorizontal, 0, movimientoVertical).normalized;
        if (direccion.magnitude >= 0.1f)
        {
            float velocidadObjetivo = estaCorriendo ? velocidadCarrera : velocidadCaminata;
            velocidadActual = Mathf.SmoothDamp(velocidadActual, velocidadObjetivo, ref velocidad.x, suavizadoDeMovimiento);

            Vector3 mover = transform.TransformDirection(direccion) * velocidadActual * Time.deltaTime;
            rigidbody.MovePosition(transform.position + mover);

            // Actualizar el parámetro de velocidad en el Animator
            animator.SetFloat("Velocidad", velocidadActual / velocidadCarrera);
        }
        else
        {
            velocidadActual = Mathf.SmoothDamp(velocidadActual, 0, ref velocidad.x, suavizadoDeMovimiento);
            animator.SetFloat("Velocidad", 0);
        }

        // Comprobar si el personaje está en el suelo
        estaEnElSuelo = Physics.CheckSphere(sueloCheck.position, 0.1f, sueloLayer);
    }

    private void Saltar()
    {
        if (estaEnElSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(Vector3.up * fuerzaDeSalto, ForceMode.Impulse);
            estaEnElSuelo = false;
        }

        // Actualizar parámetro de salto en el Animator
        animator.SetBool("IsJumping", !estaEnElSuelo);

        // Aumentar la velocidad de caída
        if (!estaEnElSuelo)
        {
            rigidbody.AddForce(Vector3.down * gravedadAdicional, ForceMode.Acceleration);
        }
    }

    private void ControlarCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse * Time.deltaTime;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & sueloLayer) != 0)
        {
            estaEnElSuelo = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & sueloLayer) != 0)
        {
            estaEnElSuelo = false;
        }
    }
}

