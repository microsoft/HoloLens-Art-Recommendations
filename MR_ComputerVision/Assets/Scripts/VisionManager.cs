using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VisionManager : MonoBehaviour {

    // the custom endpoint to make the request at
    public static string endpoint = "http://40.117.114.194:5000/endpoint";
    public AnalysedObject analysedObject;
    public Dictionary<string, Dictionary<string, string>> masterdictionary;


    [System.Serializable]
    public class Info
    {
        public string title;
        public string description;
    }

    [System.Serializable]
    public class Item
    {
        public string objectid;
        public Info[] information;
    }

    [System.Serializable]
    public class AnalysedObject
    {
        public string img_str;
        public List<int> ordering;
        public Item[] items_info;
    }

    public static VisionManager instance;

    public Texture2D tex;
    
    internal byte[] imageBytes;

    internal string imagePath;
    internal string textPath;

    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
    }

    /// <summary>
    /// Call the Computer Vision Service to submit the image.
    /// </summary>
    public IEnumerator AnalyseLastImageCaptured()
    {
        WWWForm webForm = new WWWForm();

        // gets a byte array out of the saved image
        imageBytes = GetImageAsByteArray(imagePath);
        String s = Convert.ToBase64String(imageBytes);
        webForm.AddField("image", s);

        // ResultsLabel.instance.lastLabelPlaced.GetComponent<TextMesh>().text = "sending query";

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(endpoint, webForm))
        {
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                print(unityWebRequest.error);
            }
            else
            {
                print("Finished Uploading Screenshot");
                String json_response_text = unityWebRequest.downloadHandler.text;

                analysedObject = new AnalysedObject();
                analysedObject = JsonUtility.FromJson<AnalysedObject>(json_response_text);

                masterdictionary = GetDictFromJsonText(analysedObject);

                // get the first objectid
                string current_objectid = analysedObject.items_info[0].objectid;

                // set the title
                WriteTitle(masterdictionary, current_objectid);

                // set the information
                WriteInformation(masterdictionary, current_objectid);

                // set the image from the base64 string
                WriteImageToScreenFromBase64(analysedObject.img_str);
                

            }
        }
    }

    public void SetAllInfoFromObjectIDIndex(int index, GameObject similarity_image)
    {

        // set the position of the highlight game object
        GameObject highlight_panel = similarity_image.transform.Find("HighlightPanel").gameObject;
        // GameObject highlight_panel = GameObject.Find("HighlightPanel");
        Debug.Log(highlight_panel.transform.localPosition);
        highlight_panel.transform.localPosition = new Vector3((index * 20) + 10 - 50, highlight_panel.transform.localPosition.y, highlight_panel.transform.localPosition.z);

        string current_objectid = analysedObject.items_info[index].objectid;

        // set the title
        WriteTitle(masterdictionary, current_objectid);

        // set the information
        WriteInformation(masterdictionary, current_objectid);
    }

    public void WriteImageToScreenFromBase64(string base64_string)
    {
        // update the image with the base64 string of the k nearest neighbors image

        // get the bytes from the base64 image string
        // had to do a few conversions to make it correct for the texture steps
        byte[] image_bytes = System.Text.Encoding.ASCII.GetBytes(base64_string);
        String utf_string = System.Text.Encoding.UTF8.GetString(image_bytes);
        image_bytes = Convert.FromBase64String(utf_string);

        // create a texture that can be drawn to the screen
        // the size will be overwritten by the byte data anyways
        tex = new Texture2D(2, 2);
        tex.LoadImage(image_bytes);

        // write the texture image to the correct place
        GameObject similarity_image_object = ResultsLabel.instance.lastLabelPlaced.transform.Find("SimilarityImage").gameObject;
        // GameObject similarity_image_object = GameObject.Find("SimilarityImage");
        UnityEngine.UI.RawImage similarity_raw_image = similarity_image_object.GetComponent<UnityEngine.UI.RawImage>();
        similarity_raw_image.texture = tex;
    }

    public Dictionary<string, Dictionary<string, string>> GetDictFromJsonText(AnalysedObject analysedObject)
    {
        
        Dictionary<string, Dictionary<string, string>> masterdictionary = new Dictionary<string, Dictionary<string, string>>();


        foreach (Item i in analysedObject.items_info)
        {
            Debug.Log(i.objectid);

            Dictionary<string, string> EmployeeList = new Dictionary<string, string>();

            foreach (Info info in i.information)
            {
                Debug.Log(info.title);
                Debug.Log(info.description);

                EmployeeList.Add(info.title, info.description);
            }

            masterdictionary.Add(i.objectid, EmployeeList);
        }

        return masterdictionary;

    }

    public void WriteTitle(Dictionary<string, Dictionary<string, string>> all_object_information, string objectid)
    {
        // this writes the title for the relevant objectid

        // get the title and write it to the relevant section
        GameObject title = ResultsLabel.instance.lastLabelPlaced.transform.Find("Title").gameObject;
        // GameObject title = GameObject.Find("Title");
        Text title_text = title.GetComponent<Text>();
        title_text.text = all_object_information[objectid]["title"];
    }

    public void WriteInformation(Dictionary<string, Dictionary<string, string>> all_object_information, string objectid)
    {
        // this function will get the game object and write the relevant text
        GameObject information_object = ResultsLabel.instance.lastLabelPlaced.transform.Find("Information").gameObject;
        // GameObject information_object = GameObject.Find("Information");
        Text information_text = information_object.GetComponent<Text>();

        // TODO(ethan): adjust this based on the amount of text
        // set some font attributes
        information_text.fontSize = 4;

        // this variable is used to update the actual drawing
        String information_string = "";

        // list of the relevant sections to check for
        var relevant_list = new List<string>()
        {
            "department",
            "culture",
            "artistRole",
            "objectEndDate",
            "medium",
            "creditLine",
            "geographyType",
            "classificaiton"
        };

        // iterate through the relevant sections and check if in the dictionary
        foreach (var label in relevant_list)
        {
            if (all_object_information[objectid].ContainsKey(label))
            {
                information_string += label + ": " + all_object_information[objectid][label] + "; ";
            }
        }

        // finally set the text with the relevant information
        information_text.text = information_string;

    }

    /// <summary>
    /// Returns the contents of the specified file as a byte array.
    /// </summary>
    private static byte[] GetImageAsByteArray(string imageFilePath)
    {
        FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
    }

}
