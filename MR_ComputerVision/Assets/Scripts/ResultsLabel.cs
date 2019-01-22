using System.Collections.Generic;
using UnityEngine;

public class ResultsLabel : MonoBehaviour
{
    public static ResultsLabel instance;

    public GameObject cursor;

    public Transform labelPrefab;

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
        // lastLabelPlaced = Instantiate(labelPrefab, cursor.transform.position, transform.rotation);
    }

    /// <summary>
    /// Instantiate a Label in the appropriate location relative to the Main Camera.
    /// </summary>
    public void CreateLabel()
    {
        // ethan commented this out to stay with a single label
        lastLabelPlaced = Instantiate(labelPrefab, cursor.transform.position, transform.rotation);

        // lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();

        // Change the text of the label to show that has been placed
        // The final text will be set at a later stage
        // lastLabelPlacedText.text = "Analysing...";
    }
    
}