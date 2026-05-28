using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Sample2 : MonoBehaviour
{
    [SerializeField] private int _rowCount = 10;
    [SerializeField] private int _columnCount = 10;

    private MyInputAction _inputActionAsset;
    private Image[,] _imageArray;

    private (int x, int y) _currentPosition = (0, 0);

    private void Awake()
    {
        _inputActionAsset = new MyInputAction();
        var gridLayout = GetComponent<GridLayoutGroup>();

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = _columnCount;

        _imageArray = new Image[_rowCount, _columnCount];

        for (var r = 0; r < _rowCount; r++)
        {
            for (var c = 0; c < _columnCount; c++)
            {
                var obj = new GameObject($"Cell({r}, {c})");
                obj.transform.SetParent(transform);

                var image = obj.AddComponent<Image>();
                _imageArray[r, c] = image;
                image.color = (r == 0 && c == 0) ? Color.red : Color.white;
            }
        }
    }

    private void OnEnable()
    {
        _inputActionAsset.Enable();
        _inputActionAsset.Player.Move.started += MoveNext;
        _inputActionAsset.Player.Jump.started += DisableObject;
    }

    private void OnDisable()
    {
        _inputActionAsset.Disable();
        _inputActionAsset.Player.Move.started -= MoveNext;
        _inputActionAsset.Player.Jump.started -= DisableObject;
    }
    private void DisableObject(InputAction.CallbackContext context)
    {
        var tryDisableResult = TryDisable();
        _imageArray[_currentPosition.y, _currentPosition.x].color = Color.clear;
        if (tryDisableResult != _currentPosition)
        {
            _imageArray[tryDisableResult.y, tryDisableResult.x].color = Color.red;
        }
        _currentPosition = tryDisableResult;
    }
    private (int x, int y) TryDisable()
    {
        var currentPos = _currentPosition;
        (int x, int y) nearest = currentPos;
        int minDistance = int.MaxValue;
        for (int y = 0; y < _rowCount; y++)
        {
            for (int x = 0; x < _columnCount; x++)
            {
                if (_imageArray[y, x].color == Color.clear)
                    continue;

                if ((x, y) == currentPos)
                    continue;

                int distance = Mathf.Abs(x - currentPos.x) + Mathf.Abs(y - currentPos.y);

                if (distance == 1)
                {
                    return (x, y);
                }
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = (x, y);
                }
            }
        }

        return nearest;
    }
    private void MoveNext(InputAction.CallbackContext context)
    {
        Vector2 inputForce = context.ReadValue<Vector2>();

        int dirX = (int)inputForce.x;
        int dirY = (int)inputForce.y * -1;

        var (success, nextPos) = TryMove(dirX, dirY);
        if (!success) return;
        _imageArray[_currentPosition.y, _currentPosition.x].color = Color.white;
        _currentPosition = nextPos;
        _imageArray[_currentPosition.y, _currentPosition.x].color = Color.red;
    }
    private (bool, (int x, int y)) TryMove(int dirX, int dirY)
    {
        int nextX = _currentPosition.x + dirX;
        int nextY = _currentPosition.y + dirY;
        while (nextX >= 0 && nextX < _columnCount && nextY >= 0 && nextY < _rowCount)
        {
            if (_imageArray[nextY, nextX].color == Color.white) return (true, (nextX, nextY));
            nextX += dirX;
            nextY += dirY;
        }
        return (false, _currentPosition);
    }
}