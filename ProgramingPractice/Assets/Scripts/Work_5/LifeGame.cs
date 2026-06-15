using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(GridLayoutGroup))]
public class LifeGame : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isGenerationChanging)
        {
            return;
        }

        var target = eventData.pointerCurrentRaycast.gameObject;
        if (!target.TryGetComponent<LifeGameCell>(out var cell))
        {
            return;
        }

        cell.SetState();
    }
    [SerializeField] private int _rows = 10;
    [SerializeField] private int _columns = 10;
    [SerializeField]
    [Multiline]
    private string _data = "";
    private LifeGameCell[,] _cells;
    private bool _isGenerationChanging;
    private void Awake()
    {
        var gridLayout = GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = _columns;
        _cells = new LifeGameCell[_rows, _columns];
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _columns; x++)
            {
                var obj = new GameObject($"Cell({y}, {x})");
                obj.transform.SetParent(transform);
                obj.AddComponent<Image>();
                obj.AddComponent<LifeGameCell>().Initialize(x, y);
                _cells[y, x] = obj.GetComponent<LifeGameCell>();
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _isGenerationChanging = true;
            GenerationalChange();
        }
        _isGenerationChanging = false;
    }
    /// <summary>
    /// 世代交代を行う
    /// </summary>
    private void GenerationalChange()
    {

    }
}
