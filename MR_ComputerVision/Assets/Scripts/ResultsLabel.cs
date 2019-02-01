// this will keep track of the information panel prefabs and instantiate them

using System.Collections.Generic;
using UnityEngine;

public class ResultsLabel : MonoBehaviour
{
    public static ResultsLabel instance;

    public GameObject cursor;

    // this is the prefab that will be instantiated when the user clicks the world and takes a picture
    // with the hololens
    public Transform labelPrefab;

    // keep track of the last label placed in the world in case we wan't to use that info at some point
    [HideInInspector]
    public Transform lastLabelPlaced;

    [HideInInspector]
    public TextMesh lastLabelPlacedText;

    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
    }

    public void Start()
    {
    }

    /// <summary>
    /// Instantiate a Label in the appropriate location relative to the Main Camera.
    /// </summary>
    public void CreateLabel()
    {
        // set the position of the label panel
        Vector3 vec = cursor.transform.position;
        lastLabelPlaced = Instantiate(labelPrefab, vec, transform.rotation);
    }
    
}