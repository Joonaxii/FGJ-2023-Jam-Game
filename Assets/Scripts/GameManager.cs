using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameStats GetStats => _stats;

    [SerializeField] private GameStats _stats;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BorderlessWindow.InitializeOnLoad();
        GTime.Init();
    }

    public void MinimizeWindow() => BorderlessWindow.MinimizeWindow();
    public void MaximizeWindow() => BorderlessWindow.MaximizeWindow();

    public void StartGame()
    {
        GTime.Reset();
        _stats.Reset();
    }

    public void Update()
    {
        GTime.Tick(Time.deltaTime);
        _stats.Update(GTime.GetDeltaTime(0));
    }
}
