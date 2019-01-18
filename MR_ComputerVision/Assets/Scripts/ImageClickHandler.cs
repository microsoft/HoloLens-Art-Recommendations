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

        // for some reaosn worldPosition is always 0,0,0
        // TODO(ethan): fix this bug
        //Debug.Log(eventData.pointerCurrentRaycast.worldPosition);
        

        // min and max values are 214 and 705
        float x = eventData.pressPosition.x;

        float x_normalized = (x - 214) / (705 - 214);
        Debug.Log(x_normalized);

        int objectid_index = (int)(x_normalized / 0.20);
        Debug.Log(objectid_index);

        // update with the relevant information
        GameObject.Find("Main Camera").GetComponent<VisionManager>().SetAllInfoFromObjectIDIndex(objectid_index);
    }
}
