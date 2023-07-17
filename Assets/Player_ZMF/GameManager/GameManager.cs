using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
        public static GameManager gameManager;

        private bool restartGame = false;

        private void Awake()
        {
            if (gameManager == null)
            {
                gameManager = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            EndGame();

            if (restartGame)
            {
                RestartGame();
            }
        }

        void EndGame()
        {
            if (PlayerAttribute.instance.life <= 0)
            {
                GameOver();
            }
        }

        void GameOver()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                // Restart the game
                restartGame = true;
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                // Quit the game
                Application.Quit();
            }

            // Add any additional input handling here
        }

        void RestartGame()
        {
            PlayerAttribute.instance.ResetToDefault();
            Debug.Log("ÖØÖÃ³É¹¦");

            // Reset any other necessary game state here

            restartGame = false;
            //SceneManager.LoadScene("GameScene");
        }
    }

