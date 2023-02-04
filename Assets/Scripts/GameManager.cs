using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static bool IsMaximized => !Instance._maximize;

    public static GameManager Instance { get; private set; }

    public CameraController CameraController => _cameraCtrl;
    public Player Player => _player;
    public GameBoard Board => _board;
    public InputHandler Inputs => _inputs;

    public GameStats GetStats => _stats;

    [SerializeField] private GameStats _stats;
    private Player _player;
    [SerializeField] private GameBoard _board;

    [SerializeField] private CameraController _cameraCtrl;
    [SerializeField] private AnimationCurve _cameraMoveCurve;

    private InputHandler _inputs;

    private LineRenderer _path;

    private bool _gameStarted;
    private bool _scanInProgress;

    private bool _maximize = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _inputs = new InputHandler();
        _path = transform.Find("Path").GetComponent<LineRenderer>();
        Instance = this;
        DontDestroyOnLoad(gameObject);
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        BorderlessWindow.InitializeOnLoad();
        BorderlessWindow.RestoreWindow();
#endif
        GTime.Init();

        _player = new Player();
    }

    public void MinimizeWindow()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        BorderlessWindow.MinimizeWindow();
#endif
    }
    public void MaximizeWindow()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        BorderlessWindow.MaximizeWindow();
#endif
    }
    public void RestoreWindow()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        BorderlessWindow.RestoreWindow();
#endif
    }

    public void ToggleWindowSize()
    {
        _maximize = !_maximize;
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        if(_maximize)
        {
            BorderlessWindow.RestoreWindow();
        }
        else
        {
            BorderlessWindow.MaximizeWindow();
        }
#endif
    }

    public void InitGame()
    {
        _path.positionCount = 0;

        GTime.Reset();
        _stats.Reset();

        _board.Generate(31, 31);
        _player.ResetPlayer(_board);

        _cameraCtrl.SetPosition(Vector3.zero, true);
        _cameraCtrl.SetNearFarBlend(1.0f, true);

        UpdatePath();
    }

    public IEnumerator StartGame()
    {
        float fadeDur = 1.0f;
        _cameraCtrl.SetNearFarBlend(1.0f, true);

        yield return new WaitForSeconds(1.0f);

        float t = 0;
        while (t < fadeDur)
        {
            _cameraCtrl.SetNearFarBlend(1.0f - _cameraMoveCurve.Evaluate(t / fadeDur), true);
            t += GTime.GetDeltaTime(0);
            yield return null;
        }

        _cameraCtrl.SetNearFarBlend(0, true);
        _gameStarted = true;
    }

    public void UpdatePath()
    {
        _path.positionCount = 0;

        var path = _player.GetPath;
        if (path.Count > 0)
        {

            Vector3[] positions = new Vector3[path.Count + 1];
            positions[path.Count] = _board.GridToWorld(_player.Coord);
            positions[path.Count].y = 0.1f;

            for (int i = 0; i < path.Count; i++)
            {
                positions[i] = _board.GridToWorld(path[i]);
                positions[i].y = 0.1f;
            }
            _path.positionCount = path.Count + 1;
            _path.SetPositions(positions);
        }
    }

    public void QuitGame() => Application.Quit();

    public void Update()
    {
        GTime.Tick(Time.deltaTime);

        _inputs.Update();

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

            _player.Update(delta);
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

    public void TriggerDetection()
    {

    }

    private IEnumerator _scan;
    private IEnumerator ScanSequence()
    {
        float fadeDur = 1.5f;
        float t = 0;
        while (t < fadeDur)
        {
            _cameraCtrl.SetNearFarBlend(_cameraMoveCurve.Evaluate(t / fadeDur), true);
            t += GTime.GetDeltaTime(0);
            yield return null;
        }

        _cameraCtrl.SetNearFarBlend(1.0f, true);

        int width = _board.Width;
        for (int i = 0; i < width; i++)
        {
            if (i == width - 1)
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

        fadeDur = 1.5f;
        t = 0;
        while (t < fadeDur)
        {
            _cameraCtrl.SetNearFarBlend(1.0f - _cameraMoveCurve.Evaluate(t / fadeDur), true);
            t += GTime.GetDeltaTime(0);
            yield return null;
        }

        _cameraCtrl.SetNearFarBlend(0.0f, true);
    }
}
