using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienCreep
{
    //For prototype simplicity, the GameManager is not set up as a singleton and
    //must exist in the scene. Other objects that require a gamemanager instance,
    //eg for callbacks/state checks, need to grab a reference with a
    //FindGameObjectOfType<GameManager>() call

    //This class controls various game states and events, as well as storing the score

    public class GameManager : MonoBehaviour
    {
        //enum for State references 
        public enum State { undefined, inPlay, inMenu, inWin, inLose};

        //events
        public event Action OnStartGame, OnLoseGame, OnWinGame;
        public event Action OnScoreUpate, OnEnemyDestroyed;

        //public fields
        [HideInInspector]
        public State curState;                                  //holds the current state

        //private fields
        private int _score;                                     //backing field for the Score property

        //Properties
        public int Score
        {
            get { return _score; }
            set 
            { 
                _score = value;
                //invoke any callbacks when the score changes - used by UIManager
                if (OnScoreUpate != null)
                    OnScoreUpate();
            }
        }

        private void Awake()
        {
            //set to portrait mode - also forced in Player Settings
            Screen.orientation = ScreenOrientation.Portrait;
            //set initial state
            curState = State.inMenu;
        }

        //Start game is called from the 'play' button unity events, set in inspector.
        public void StartGame()
        {
            //reset the score
            Score = 0;
            //set the state
            curState = State.inPlay;
            //invoke any callbacks
            if (OnStartGame != null)
                OnStartGame();
        }

        public void GameOverWin()
        {
            //set the state
            curState = State.inWin;
            //invoke any callbacks
            if (OnWinGame != null)
                OnWinGame();
        }

        public void GameOverLose()
        {
            //set the state
            curState = State.inLose;
            //invoke any callbacks
            if (OnLoseGame != null)
                OnLoseGame();
        }

        public void AddToScore(int value)
        {
            //update the score (which invokes it's own callbacks in setter)
            Score += value;
        }

        public void EnemyDestroyed()
        {
            //coroutine used to wait until end of frame before invoking any callbacks
            //to ensure the Destory() method has completed on the enemy object. 
            StartCoroutine(EnemyDestroyedEOF());
        }

        private IEnumerator EnemyDestroyedEOF()
        {
            //wait until end of frame
            yield return null; 
            //invoke any callbacks
            if (OnEnemyDestroyed != null)
                OnEnemyDestroyed();
        }
    }

}
