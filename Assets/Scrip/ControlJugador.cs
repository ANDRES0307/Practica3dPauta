using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    public float velocidadMovimiento = 5.0f;
    public float velocidadRotacion = 200.0f;
    public float x,y;
    private Animator animator;
    private Rigidbody rigidbody;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal") * velocidadRotacion * Time.deltaTime;
        y = Input.GetAxis("Vertical") * velocidadMovimiento *Time.deltaTime;
        Mover(x, y);

    }

    private void FixedUpdate()
    {
        //Mover(x, y);
    }

    private void Mover(float x, float y)
        {
            transform.Rotate(0,x,0);
            transform.Translate(0,0,y);
            animator.SetFloat("VelX",x);
            animator.SetFloat("VelY",y*100);

        }

}