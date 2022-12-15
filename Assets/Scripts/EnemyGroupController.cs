using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienCreep
{
    //Enemies group - holds the wave of enemies, and positions each one in a grid relative to the 
    //origin of this gameobject taking bottom Left as x=0, y=0, using world space units as grid positions.
    //This object then moves the whole group as children. The transform position.x limits are calculated based
    //on the enemies array [columns,rows] where left and right most columns that contain enemies are
    //calculated, as well as the lowest row, to calculate when an enemy reaches the 'ground'. 

    public class EnemyGroupController : MonoBehaviour
    {
        //public fields        
        public int rows = 5;                                    //grid of enemies, number of rows/columns
        public int columns = 8;
        public GameObject enemyShipPrefab;                      //reference to the enemy prefab
        public float speed;                                     //how fast the enemy group moves
        public float yStartPos;                                 //world y position of the group (lowest point)
        public int yMinForGameOver = 1;                         //world y position that triggers game over
        public float yMoveDistance = 0.5f;                      //y distance when group moves down

        //private fields
        private GameManager _gameManager;                       //refernce to the GameManager instance
        private int _xDirection = 1, _oldXDirection = 1;        //current x direction : 1 = right, -1 = left, 0 = no x movement
        private float _yDirection = 0;                          //current y direction : -1 = down, 0 = no y movement
        private GameObject[,] _enemyGroup;                      //the array of instantiated enemy object references
        private int _leftLimit, _rightLimit, _lowerLimit;       //the relative x minimum and maximum, and y minimum for this transform
        private float _xMin, _xMax;                             //the world space x minimum and maximum for moving this group
        private float _oldYPos;                                 //stores the position at the start of a y direction move, for distance checking
        private int _totalEnemiesRemaining;                     //total enemies yet to be destroyed, if 0 we have a winner
        

        private void OnEnable()
        {
            //subscribe to required events on the _gameManager object
            _gameManager.OnEnemyDestroyed += CalcGroupBoundaries;
            _gameManager.OnEnemyDestroyed += ReduceCount;
            _gameManager.OnStartGame += ResetGroup;
        }

        private void OnDisable()
        {
            //unsubscribe from events as required
            _gameManager.OnEnemyDestroyed -= CalcGroupBoundaries;
            _gameManager.OnEnemyDestroyed -= ReduceCount;
            _gameManager.OnStartGame -= ResetGroup;
        }

        private void Awake()
        {
            //find the GameManager in hte scene
            _gameManager = FindObjectOfType<GameManager>();
        }

        private void Start()
        {
            ResetGroup();
        }

        private void ResetGroup()
        {
            //initialisation of various variables;
            _xDirection = 1;
            _oldYPos = yStartPos;

            //set the world postion for the enemy group (this object)
            transform.position = new Vector3(0, yStartPos, 0);

            CreateEnemyGroup();
            CalcGroupBoundaries();
        }

        private void CreateEnemyGroup()
        {
            //create a new enemies array if required
            if (_enemyGroup == null)
                _enemyGroup = new GameObject[columns, rows];

            //run through the array, adding enemies by columns/rows and destroying any
            //existing enemies e.g. restarting the game.
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    //Before instantiating and new object, destroy the old enemy if one still exists
                    if (_enemyGroup[i, j] != null)
                        Destroy(_enemyGroup[i, j].gameObject);
                    
                    //calculate the world position for the next enemy object
                    Vector2 pos = new Vector2(transform.position.x + i + 0.5f, transform.position.y + j + 0.5f);

                    //instantiate and postition the new enemy
                    GameObject enemy = Instantiate(enemyShipPrefab, pos, Quaternion.identity, this.transform);
                    
                    //log the reference in the enemy array
                    _enemyGroup[i, j] = enemy;
                }
            }

            //store the total enemies for win state checking
            _totalEnemiesRemaining = rows * columns;
        }

        private void CalcGroupBoundaries()
        {
            //this method finds the left/right/lowermost enemies by checking for
            //object references in the _enemyGroup array. This provides a left and right
            //column indexes that can be used directly to check if the group has reached the left
            //or right of the play area. Relies on 1:1 enemy per worldspace unit

            //set the left, right and lower limits to 'not found' value;
            _leftLimit = _rightLimit = _lowerLimit = -1;

            //check the array of enemies for the left most occurrence ( [0,0] = bottom left)
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (_enemyGroup[i, j] != null && _leftLimit == -1) //this will be the first enemy found
                    {
                        _leftLimit = i; //set to leftLimit to the found column index
                        break; //and stop checking 
                    }
                }

                if (_leftLimit > -1) //we've found an enemy in the column so stop looking
                    break;
            }

            //do the same the the right limit - for loop starts at the highest column index and decrements
            //check the array of enemies for the right most occurrence
            for (int i = columns - 1; i >= 0; i--)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (_enemyGroup[i, j] != null && _rightLimit == -1) //this will be the first enemy found
                    {
                        _rightLimit = i; //set to the found column
                        break; //no need to keep checking the column
                    }
                }

                if (_rightLimit > -1) //we've found an enemy so stop looking
                    break;
            }

            //do the same the the lowerLimit, this time by rows first
            //check the array of enemies for the lowest most occurrence
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    if (_enemyGroup[i, j] != null && _lowerLimit == -1) //this will be the first enemy found
                    {
                        _lowerLimit = j; //set the the lowest row
                        break; //no need to keep checking the column
                    }
                }

                if (_lowerLimit > -1) //we've found an enemy so stop looking
                    break;
            }

            //set the world space X minimum and maximum for using with this transform.position.x
            //relies on play area being positioned in worldspace with bottom/left = 0,0 and being 
            //10 world units wide
            _xMin = 0 - _leftLimit; 
            _xMax = 9 - _rightLimit; //our world space play area is 10 units wide (units 0 - 9).
        }

        private void PostionEnemyGroup()
        {
            //Calculate the worldposition of the group using current position plus calculated movement
            float _xPos = transform.position.x + _xDirection * speed * Time.deltaTime;
            float _yPos = transform.position.y + _yDirection * speed * Time.deltaTime;

            //set the position
            transform.position = new Vector3(_xPos, _yPos, 0);
        }

        private void CalculateMovementDirection()
        {
            //Game starts with group moving right, once the rightmost (maximum x) position is hit
            //stop moving in x, and move in y (downwards), same for leftmost (minimum x) when moving left.
            //When moving down the max y movement in set in inspector, and when y limit reached, we revert to
            //the opposite x direction and start moving in x again.

            //check if moving right and _xMax reached
            if (_xDirection > 0 && transform.position.x >= _xMax)
            {
                if (_gameManager.curState == GameManager.State.inPlay)
                {   //move down
                    _yDirection = -1;
                    _oldXDirection = _xDirection;
                    _xDirection = 0;
                    _oldYPos = transform.position.y;
                }
                else  //not inPlay so just change x direction
                {
                    _xDirection = -1; //move left
                }
            }

            //check if moving left and _xMin reached
            if (_xDirection < 0 && transform.position.x <= _xMin)
            {
                if (_gameManager.curState == GameManager.State.inPlay)
                {
                    //move down
                    _yDirection = -1;
                    _oldXDirection = _xDirection;
                    _xDirection = 0;
                    _oldYPos = transform.position.y;
                }
                else //not inPlay so just change x direction
                {
                    _xDirection = 1; //move right 
                }
            }

            //check if moving in y and we've moved yMoveDistance units;
            //need to remember what x direction we were moving in and reverse
            if (_yDirection != 0 && _oldYPos - transform.position.y >= yMoveDistance)
            {
                //stop moving down
                _yDirection = 0;

                //move in the opposite x direction than before
                _xDirection = -_oldXDirection;
            }
        }

        private void CheckGameOver()
        {
            //check the win condition : _totalEnemiesRemaining gets updated via OnEnemyDestroyed callback
            if (_totalEnemiesRemaining <= 0)
                _gameManager.GameOverWin();
            //else check lose condition : this transforms postition has reached the y lower limit 
            else if (transform.position.y + _lowerLimit <= yMinForGameOver)
            {
                _xDirection = 0;
                _yDirection = 0;
                _gameManager.GameOverLose();
            }
        }

        private void ReduceCount()
        {
            //called by GameManager.OnEnemyDestroyed callback
            _totalEnemiesRemaining--;
        }

        private void Update()
        {
            //Update loop for this group of enemies
            CalculateMovementDirection();
            PostionEnemyGroup();
            CheckGameOver();
        }
    }
}