using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    private Vector2 moveVector;

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Rigidbody2D rb;

    private void FixedUpdate()
    {
        //float xDirection = Input.GetAxis("Horizontal");
        //float yDirection = Input.GetAxis("Vertical");

        //Vector2 moveDirection = new Vector2 (xDirection, yDirection);

        //transform.position += (Vector3)moveDirection * speed;

        //var dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);
    }
}
