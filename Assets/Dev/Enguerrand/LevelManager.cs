using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject _mainMenuUI;
    [SerializeField] GameObject _gameOverUI;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (_mainMenuUI.activeInHierarchy == true)
            {
                loadGame();
            }
            string key = Input.inputString.ToUpper();
            var button = InputController.Instance.GetKey(key);
            if (button.Color == PlayerController.ButtonColor.BLUE)
            {
                retry();
            }
            if (button.Color == PlayerController.ButtonColor.RED)
            {
                quit();
            }
        }
    }

    public void loadGame()
    {

        SceneManager.LoadScene("Antoine");
    }

    public void retry()
    {
        if (_gameOverUI.activeInHierarchy == true)
        {
            SceneManager.LoadScene("Antoine");
        }
    }

    public void quit()
    {
        if (_gameOverUI.activeInHierarchy == true)
        {
            SceneManager.LoadScene("proto");
        }
    }

}
