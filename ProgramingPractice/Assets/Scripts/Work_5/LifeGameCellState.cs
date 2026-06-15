using UnityEngine;

/// <summary>
/// ライフゲームのセルの状態を表す列挙型
/// </summary>
public enum LifeGameCellState
{
    [InspectorName("Dead")]
    Dead = 0,
    [InspectorName("Alive")]
    Alive = 1
}
