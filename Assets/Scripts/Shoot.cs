using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AlienCreep
{
    public class Shoot : MonoBehaviour
    {
        //this class should be attached to the gun object. It instantiates a bullet prefab at this transform's
        //position at a regular interval, set in the inspector. Bullet's handle their own existence

        //public fields
        public GameObject bullet;                                   //reference to the Bullet prefab for instantiation
        public float shootInterval = 1f;                            //how long between bullet shots

        //private fields
        private GameManager _gameManager;                           //reference to the GameManager instance

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();
            
            if (bullet == null)
            {
                Debug.LogError("## Shoot.cs : Check inspector references!");
                Debug.Break();
            }
        }

        private void OnEnable()
        {
            _gameManager.OnStartGame += StartShooting;
            _gameManager.OnStartGame += RemoveAllBullets;
        }

        private void OnDisable()
        {
            _gameManager.OnStartGame -= StartShooting;
            _gameManager.OnStartGame -= RemoveAllBullets;
        }

        private void RemoveAllBullets()
        {
            //Removes all existing bullets eg after restarting from win/lose
            //Find all bulletContollers in the scene - called from the GameManager.OnStartGame callback
            BulletController[] _bulletControllers = FindObjectsOfType<BulletController>();
            //...and destroy the gameobject they are on.
            foreach (BulletController bc in _bulletControllers)
                Destroy(bc.gameObject);
        }

        private void StartShooting()
        {
            //called from the GameManager.OnStartGame - invokes the ConstantShoot coroutine 
            StartCoroutine(ConstantShoot(shootInterval));
        }

        private IEnumerator ConstantShoot(float shotInterval)
        {
            //keeps shooting while the GameManager.State is 'inPlay'
            float timer = 0;

            while (_gameManager.curState == GameManager.State.inPlay)
            {
                timer += Time.deltaTime;

                if (timer >= shotInterval)
                {
                    Instantiate(bullet, transform.position, Quaternion.identity);
                    timer = 0;
                }
                //wait for the end of frame
                yield return null;
            }

            //in case the while loop never runs
            yield return null;
        }
    }
}
