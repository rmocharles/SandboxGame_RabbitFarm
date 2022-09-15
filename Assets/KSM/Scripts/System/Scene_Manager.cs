using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    private static Scene_Manager instance;

    #region Scene
    private const string LOGIN = "1. Login";
    private const string FARM = "2. Farm";
    private const string MART = "3. Mart";
    #endregion

    public enum GameState { Login, Farm, Mart };
    public GameState gameState;
    void Awake()
    {
        if (!instance) instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static Scene_Manager GetInstance()
    {
        if (instance == null)
            return null;
        return instance;
    }

    public void ChangeState(GameState state)
    {
        gameState = state;
        switch (gameState)
        {
            case GameState.Login:
                ChangeScene(LOGIN);
                break;
            case GameState.Farm:
                ChangeScene(FARM);
                break;
            case GameState.Mart:
                ChangeScene(MART);
                break;
        }
    }

    private void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
