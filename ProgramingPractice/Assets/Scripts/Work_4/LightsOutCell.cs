using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LightsOutCell : MonoBehaviour, IPointerClickHandler
{
    public event Action<LightsOutCell> OnClicked;

    public (int x, int y) Position { get; private set; }
    public bool IsOn { get; private set; }

    private Image _image;

    public void Initialize(int x, int y)
    {
        _image = GetComponent<Image>();
        Position = (x, y);
        SetState(false);
    }

    public void Toggle() => SetState(!IsOn);

    public void SetState(bool isOn)
    {
        IsOn = isOn;
        _image.color = isOn ? Color.white : Color.black;
    }

    public void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(this);
}
