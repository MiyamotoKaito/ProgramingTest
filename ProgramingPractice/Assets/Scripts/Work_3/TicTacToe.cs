using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    private const int Size = 3;

    private Image[,] _cells;

    [SerializeField]
    private Sprite _circle = null;

    [SerializeField]
    private Sprite _cross = null;

    private MyInputAction _inputActions;
    private (int x, int y) _currentPos = (0, 0);

    private bool _is1P = true;
    private void Awake()
    {
        _inputActions = new();

        _cells = new Image[Size, Size];
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                var obj = new GameObject($"{y} {x}");
                obj.transform.SetParent(transform);
                var cellImage = obj.AddComponent<Image>();
                _cells[y, x] = cellImage;
            }
        }
    }
    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Player.Move.started += Selcet;
        _inputActions.Player.Jump.started += Paset;
    }
    private void OnDisable()
    {
        _inputActions.Player.Move.started -= Selcet;
        _inputActions.Player.Jump.started -= Paset;
        _inputActions.Disable();
    }
    private void Selcet(InputAction.CallbackContext context)
    {
        Vector2 inputForce = context.ReadValue<Vector2>();

        var tryResult = TrySelecet((int)inputForce.x, (int)inputForce.y);

        if (!tryResult.isSuccess) return;

        _cells[_currentPos.y, _currentPos.x].color = Color.white;
        _currentPos = tryResult.nextPos;
        _cells[_currentPos.y, _currentPos.x].color = Color.cyan;
    }
    private void Paset(InputAction.CallbackContext context)
    {
        if (_cells[_currentPos.y, _currentPos.x].sprite == null)
        _cells[_currentPos.y, _currentPos.x].sprite = _is1P ? _circle : _cross;
    }
    private (bool isSuccess, (int x, int y) nextPos) TrySelecet(int dirX, int dirY)
    {
        int nextX = _currentPos.x + dirX;
        int nextY = _currentPos.y + dirY;
        while (nextX >= 0 && nextX < Size && nextY >= 0 && nextY < Size)
        {
            if (_cells[nextY, nextX].color == Color.white ||
                _cells[nextY, nextX].sprite == null
                ) return (true, (nextX, nextY));
            nextX += dirX;
            nextY += dirY;
        }
        return (false, _currentPos);
    }
}