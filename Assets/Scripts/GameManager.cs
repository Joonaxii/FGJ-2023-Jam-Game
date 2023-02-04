using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CameraController CameraController => _cameraCtrl;
    public Player Player => _player;
    public GameBoard Board => _board;

    public GameStats GetStats => _stats;

    [SerializeField] private GameStats _stats;
    private Player _player;
    [SerializeField] private GameBoard _board;

    [SerializeField] private CameraController _cameraCtrl;

    private bool _gameStarted;
    private bool _scanInProgress;

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

        _player = new Player();
    }

    public void Start()
    {
        StartGame();
    }

    public void MinimizeWindow() => BorderlessWindow.MinimizeWindow();
    public void MaximizeWindow() => BorderlessWindow.MaximizeWindow();

    public void StartGame()
    {
        GTime.Reset();
        _stats.Reset();

        _board.Generate(31, 31);

        _gameStarted = true;
        _cameraCtrl.SetPosition(Vector3.zero, true);
    }

    public void Update()
    {
        GTime.Tick(Time.deltaTime);

        if (_gameStarted)
        {
            float delta = GTime.GetDeltaTime(0);
            _cameraCtrl.Update(delta);

            _stats.Update(delta);
            if (!_scanInProgress)
            {
                if (_stats.UpdateScan(delta))
                {
                    StartScan();
                }
            }
        }
    }

    public void StartScan()
    {
        _scanInProgress = true;

        if (_scan != null)
        {
            StopCoroutine(_scan);
        }

        _scan = ScanSequence();
        StartCoroutine(_scan);
    }

    private IEnumerator _scan;
    private IEnumerator ScanSequence()
    {
        int width = _board.Width;
        for (int i = 0; i < width; i++)
        {
            if(i == width - 1)
            {
                yield return _board.BeginScan(i);
            }
            else
            {
                StartCoroutine(_board.BeginScan(i));
            }
            yield return new WaitForSeconds(0.5f);
        }
        _scanInProgress = false;
    }
}
