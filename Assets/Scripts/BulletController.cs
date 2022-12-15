using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienCreep
{  
    public class BulletController : MonoBehaviour
    {
        //a simple class attached to a bullet prefab that, when instantiated, moves the bullet at a constant
        //speed in a +ve y direction until it collides with anything - either an enemy or the 'TopCollider'

        //public fields
        public float speed = 1f;                         //speed control for the bullet movement

        void Update()
        {
            float newYPos = transform.position.y + Time.deltaTime * speed;
            transform.position = new Vector2(transform.position.x, newYPos);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(this.gameObject);
        }
    }
}