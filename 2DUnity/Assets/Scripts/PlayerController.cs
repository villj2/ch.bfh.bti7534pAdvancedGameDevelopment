using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
        public float maxSpeed = 400f;
        bool facingRight = true;
        Rigidbody2D rigidbody2D;
        Animator anim;

        void Start () {
            rigidbody2D = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }
	
        void Update () {
            float move = Input.GetAxis("Horizontal");
            anim.SetFloat("Speed", Mathf.Abs(move));
            rigidbody2D.velocity = new Vector2(move * Time.deltaTime * maxSpeed, 
                                               rigidbody2D.velocity.y);
            if (move < 0 && facingRight ||
                move > 0 && !facingRight)
                Flip();
        }
    
        // flip the character by scaling it by -1 along the x axis
        void Flip()
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = !facingRight;
        }
    }