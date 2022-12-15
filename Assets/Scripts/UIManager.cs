using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AlienCreep
{
    //This class controls the play button and UI text elements and uses GameManager callbacks
    //to update the UI as required
    
    public class UIManager : MonoBehaviour
    {
        //public fields
        public GameObject btnPlay;                                  //reference to the Play button object
        public GameObject txtMessages;                              //reference to a Text object used for simple title/win/lose messages
        public GameObject txtScore;                                 //reference to a Text object used for showing the score 

        //private fields
        private GameManager _gameManager;                           //reference to the GameManager instance

        private void OnEnable()
        {
            //subscribe to the required callbacks
            _gameManager.OnStartGame += UIPlaying;
            _gameManager.OnWinGame += UIWin;
            _gameManager.OnLoseGame += UILose;
            _gameManager.OnScoreUpate += UIScore;
        }

        private void OnDisable()
        {
            //unsubscribe as required
            _gameManager.OnStartGame -= UIPlaying;
            _gameManager.OnWinGame -= UIWin;
            _gameManager.OnLoseGame -= UILose;
            _gameManager.OnScoreUpate -= UIScore;
        }

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();

            //check for requred object references
            if (btnPlay == null
             || txtMessages == null
             || txtScore == null)
            {
                Debug.LogError("## UIManager.cs : Check inspector references!");
                Debug.Break();
            }
        }

        private void Start()
        {
            //set the initial states of the UI elements
            UIStart();
        }

        private void UIStart()
        {
            //at the start of the game, show the button and the game name
            btnPlay.SetActive(true);
            txtMessages.SetActive(true);
            txtMessages.GetComponent<TextMeshProUGUI>().text = "ALIEN CREEP";
            txtScore.GetComponent<TextMeshProUGUI>().text = string.Format("SCORE : {0}", _gameManager.Score);
        }

        private void UIPlaying()
        {
            //when playing, we don't want the button or any message
            btnPlay.SetActive(false);
            txtMessages.SetActive(false);
        }

        private void UIWin()
        {
            //when winner, show the play button and appropraite message
            btnPlay.SetActive(true);
            txtMessages.SetActive(true);
            txtMessages.GetComponent<TextMeshProUGUI>().text = "YOU WIN!";
        }

        private void UILose()
        {
            //when loser, show the play button and appropraite message
            btnPlay.SetActive(true);
            txtMessages.SetActive(true);
            txtMessages.GetComponent<TextMeshProUGUI>().text = "YOU LOSE!";
        }

        private void UIScore()
        {
            //update the score - invoked by GameManager callback when Score changes
            txtScore.GetComponent<TextMeshProUGUI>().text = string.Format("SCORE : {0}", _gameManager.Score);
        }
    }
}
