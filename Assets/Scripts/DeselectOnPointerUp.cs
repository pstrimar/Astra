using UnityEngine;
using UnityEngine.EventSystems;

public class DeselectOnPointerUp : MonoBehaviour, IPointerUpHandler
{
    // If player clicks "Continue" on dialogue with mouse instead of hitting Enter, 
    // this will remove focus every time so player controls work after dialogue without clicking in scene
    public void OnPointerUp(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
