using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienCreep
{
    public class EnemyShipCollision : MonoBehaviour
    {
        //Class attached to an enemy prefab that stores it's points value and 
        //destroys itself when hit by anything. Uses gameManager methods to log it's 
        //destruction and update the score (which in-turn invoke any callbacks required by
        //other classes

        //public fields
        public int points = 100;                                //points value when we get destroyed

        //private fields
        private GameManager _gameManager;                       //reference to the GameManger instance

        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);

            _gameManager.EnemyDestroyed();
            _gameManager.AddToScore(points);
        }

    }
}
