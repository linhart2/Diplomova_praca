using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public GameObject item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    #region IDropHandler implementation
    public void OnDrop(PointerEventData eventData)
    {
		if (!item) {
			DragHandeler.itemBeingDragged.transform.SetParent (transform);
			ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
		} else if (item && transform.GetChildCount () == 1 && transform.GetChild(0).GetComponent<DragHandeler>().enabled == true) {
			transform.GetChild (0).transform.SetParent (DragHandeler.startParent);
			DragHandeler.itemBeingDragged.transform.SetParent (transform);
			ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());
		}
    }
    #endregion
}
