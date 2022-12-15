using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienCreep
{
    public class GunController : MonoBehaviour
    {
        //The main gun object has this class and a Shoot class. This class controls the guns position
        //during play

        //private fields
        private GameManager _gameManager;                           //reference to the GameManager instance
        private float _xPos;                                        //the x position of the gun
        private Camera _mainCam;                                    //reference cache for Camera.main for ScreenToWorldSpace checking

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();            
        }

        private void OnEnable()
        {
            _gameManager.OnStartGame += ResetPosition;
        }

        private void OnDisable()
        {
            _gameManager.OnStartGame -= ResetPosition;
        }

        private void ResetPosition()
        {
            _xPos = _mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, 0)).x;
            transform.position = new Vector2(_xPos, transform.position.y);
        }

        void Start()
        {
            _mainCam = Camera.main;
            ResetPosition();
        }

        void Update()
        {
            //using GetMouseButton(0) is also simulated by primary touch point on iOS/Android
            if (Input.GetMouseButton(0)
             && _gameManager.curState == GameManager.State.inPlay)
            {
                //if we're playing, calculate the world x position based on mouse (or touch) position
                _xPos = _mainCam.ScreenToWorldPoint(Input.mousePosition).x;
                //position the gun's x position accordingly
                transform.position = new Vector2(_xPos, transform.position.y);
            }
        }
    }
}
