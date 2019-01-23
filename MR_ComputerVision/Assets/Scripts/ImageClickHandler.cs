using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class ImageClickHandler : MonoBehaviour, IPointerClickHandler {

    void Start()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        float x = localCursor[0];

        // for some reaosn worldPosition is always 0,0,0
        // TODO(ethan): fix this bug
        //Debug.Log(eventData.pointerCurrentRaycast.worldPosition);

        //Debug.Log("game position");
        //Debug.Log(transform.position);
        // min and max values are 214 and 705
        //float x = eventData.pressPosition.x;
        //Debug.Log(x);

        float x_normalized = (x + 50) / (100);
        //Debug.Log(x_normalized);

        int objectid_index = (int)(x_normalized / 0.20);
        //Debug.Log(objectid_index);

        // update with the relevant information
        GameObject.Find("Main Camera").GetComponent<VisionManager>().SetAllInfoFromObjectIDIndex(objectid_index, this.gameObject);
    }
}
