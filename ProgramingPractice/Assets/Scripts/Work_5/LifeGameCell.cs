using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ライフゲームのセルを表すクラス
/// </summary>
public class LifeGameCell : MonoBehaviour
{
    public (int x, int y) Position => _position;
    /// <summary>
    /// セルを初期化する
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Initialize(int x, int y)
    {
        _position = (x, y);
        _state = LifeGameCellState.Alive;
        _image = GetComponent<Image>();
    }
    /// <summary>
    /// セルの状態を変更する
    /// </summary>
    public void SetState()
    {
        _state = _state == LifeGameCellState.Alive ? LifeGameCellState.Dead : LifeGameCellState.Alive;
        _image.color = _state == LifeGameCellState.Alive ? Color.white : Color.black;
    }
    private Image _image;
    private LifeGameCellState _state;
    private (int x, int y) _position = (0, 0);
}
