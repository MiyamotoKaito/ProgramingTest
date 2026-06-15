using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class LightsOut : MonoBehaviour
{
    [SerializeField]
    private int _rows = 5;

    [SerializeField]
    private int _columns = 5;

    [SerializeField]
    private TMP_Text _movesText = null;

    [SerializeField]
    private TMP_Text _timeText = null;

    [SerializeField]
    private TMP_Text _resultText = null;

    private LightsOutCell[,] _cells;

    // チート用：このセル群を押せば必ずクリアできる（押す順番は問わない）
    private readonly HashSet<(int x, int y)> _solution = new();

    private int _moveCount;
    private float _startTime;
    private bool _isCleared;

    private void Start()
    {
        CreateBoard();
        SetupRandomBoard();

        _startTime = Time.time;
        _moveCount = 0;
        UpdateMovesText();
        LogSolution();
    }

    private void Update()
    {
        if (_isCleared) return;
        if (_timeText != null) _timeText.text = $"時間: {Time.time - _startTime:F1}秒";
    }

    private void CreateBoard()
    {
        var grid = GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = _columns;

        _cells = new LightsOutCell[_rows, _columns];
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var obj = new GameObject($"{y} {x}");
                obj.transform.SetParent(transform, false);
                obj.AddComponent<Image>();

                var cell = obj.AddComponent<LightsOutCell>();
                cell.Initialize(x, y);
                cell.OnClicked += OnCellClicked;
                _cells[y, x] = cell;
            }
        }
    }

    /// <summary>
    ///     盤面をランダムにセットアップする。ただし以下の条件を満たすようにする。
    ///     - クリア可能であること
    ///     - 初期状態が全て消灯ではないこと
    ///     - 1手でクリアできないこと
    /// </summary>
    private void SetupRandomBoard()
    {
        // 全消灯（クリア状態）からランダムなセルを押して盤面を作るため、必ずクリア可能になる
        const int MaxAttempts = 100;
        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            ResetBoard();

            var candidates = new List<(int x, int y)>();
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++) candidates.Add((x, y));
            }

            // 同じセルを2回押すと元に戻るため、重複なしで選ぶ
            int presses = Random.Range(2, candidates.Count + 1);
            for (int i = 0; i < presses; i++)
            {
                int index = Random.Range(0, candidates.Count);
                var pos = candidates[index];
                candidates.RemoveAt(index);

                ApplyPress(pos.x, pos.y);
                _solution.Add(pos);
            }

            if (!IsUniform() && !CanClearInOneMove()) return;
        }
        Debug.LogWarning("条件を満たす盤面を生成できませんでした。行数・列数を見直してください。");
    }

    private void ResetBoard()
    {
        _solution.Clear();
        foreach (var cell in _cells) cell.SetState(false);
    }

    private void OnCellClicked(LightsOutCell cell)
    {
        if (_isCleared) return;

        var (x, y) = cell.Position;
        ApplyPress(x, y);
        _moveCount++;
        UpdateMovesText();

        // 解法の更新：解に含まれるセルを押したら消し、含まれないセルなら追加
        if (!_solution.Remove((x, y))) _solution.Add((x, y));
        LogSolution();

        if (IsAllOff()) Clear();
    }

    private void ApplyPress(int x, int y)
    {
        ToggleIfExists(x, y);
        ToggleIfExists(x + 1, y);
        ToggleIfExists(x - 1, y);
        ToggleIfExists(x, y + 1);
        ToggleIfExists(x, y - 1);
    }

    private void ToggleIfExists(int x, int y)
    {
        if (x < 0 || x >= _columns || y < 0 || y >= _rows) return;
        _cells[y, x].Toggle();
    }

    private bool IsAllOff()
    {
        foreach (var cell in _cells)
        {
            if (cell.IsOn) return false;
        }
        return true;
    }

    private bool IsUniform()
    {
        bool first = _cells[0, 0].IsOn;
        foreach (var cell in _cells)
        {
            if (cell.IsOn != first) return false;
        }
        return true;
    }

    private bool CanClearInOneMove()
    {
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                if (WouldClear(x, y)) return true;
            }
        }
        return false;
    }

    private bool WouldClear(int pressX, int pressY)
    {
        // 押した十字範囲だけが点灯している場合のみ、その1手で全消灯になる
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                bool inCross = (x == pressX && Mathf.Abs(y - pressY) <= 1) ||
                               (y == pressY && Mathf.Abs(x - pressX) <= 1);
                if (_cells[y, x].IsOn != inCross) return false;
            }
        }
        return true;
    }

    private void Clear()
    {
        _isCleared = true;
        float clearTime = Time.time - _startTime;

        string result = $"クリア！ 時間: {clearTime:F1}秒 / 手数: {_moveCount}手";
        Debug.Log(result);
        if (_resultText != null) _resultText.text = result;
        if (_timeText != null) _timeText.text = $"時間: {clearTime:F1}秒";
    }

    private void UpdateMovesText()
    {
        if (_movesText != null) _movesText.text = $"手数: {_moveCount}手";
    }

    private void LogSolution()
    {
        if (_solution.Count == 0)
        {
            Debug.Log("解法（チート）: 残り0手");
            return;
        }

        var sb = new StringBuilder($"解法（チート）: 残り{_solution.Count}手 → ");
        foreach (var (x, y) in _solution) sb.Append($"({x}, {y}) ");
        Debug.Log(sb.ToString());
    }
}
