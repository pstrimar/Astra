using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeselectOnPointerUp : MonoBehaviour, IPointerUpHandler
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
