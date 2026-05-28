using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    [SerializeField] private int _length = 10;

    private Image[] _images;
    private int _currentNumber;

    private void Start()
    {
        _images = new Image[_length];

        for (int i = 0; i < _length; i++)
        {
            var obj = new GameObject($"Cell{i}");

            // UI階層へ追加
            obj.transform.SetParent(transform, false);

            // Image追加
            var image = obj.AddComponent<Image>();

            // 最初のやつだけ赤
            if (i == 0)
            {
                image.color = Color.red;
                _currentNumber = 0;
            }
            else
            {
                image.color = Color.white;
            }

            _images[i] = image;
        }
    }

    private void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard == null) return;

        // 左移動
        if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            MoveLeft();
        }
　   
        // 右移動
        if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            MoveRight();
        }

        // 削除して次へ
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            NextStep(_currentNumber);
        }
    }

    public void MoveLeft()
    {
        if (_currentNumber <= 0) return;

        _images[_currentNumber].color = Color.white;

        _currentNumber--;

        _images[_currentNumber].color = Color.red;
    }

    public void MoveRight()
    {
        if (_currentNumber >= _images.Length - 1) return;

        _images[_currentNumber].color = Color.white;

        _currentNumber++;

        _images[_currentNumber].color = Color.red;
    }

    /// <summary>
    /// セルを削除（透明化）
    /// </summary>
    private void DeleteObject(int index)
    {
        var color = _images[index].color;
        color.a = 0;
        _images[index].color = color;
    }

    /// <summary>
    /// 次の生きているセルへ進む
    /// </summary>
    private void NextStep(int currentNumber)
    {
        // 現在セル削除
        DeleteObject(currentNumber);

        // 次を探す
        for (int i = currentNumber + 1; i < _images.Length; i++)
        {
            // alphaが0じゃない = 生きている
            if (_images[i].color.a != 0)
            {
                _currentNumber = i;
                _images[i].color = Color.red;
                return;
            }
        }

        // 前方向を探す
        BackStep(currentNumber - 1);
    }

    /// <summary>
    /// 後ろ方向に生きているセルを探す
    /// </summary>
    private void BackStep(int currentNumber)
    {
        for (int i = currentNumber; i >= 0; i--)
        {
            if (_images[i].color.a != 0)
            {
                _currentNumber = i;
                _images[i].color = Color.red;
                return;
            }
        }

        Debug.Log("全部削除されました");
    }
}