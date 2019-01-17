using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;

public class VisionManager : MonoBehaviour {

    [System.Serializable]
    public class TagData
    {
        public string name;
        //public float confidence;
    }

    [System.Serializable]
    public class AnalysedObject
    {
        public string img_str;
        //public string requestId;
        //public object metadata;
    }

    public static VisionManager instance;

    public Texture2D tex;
    public bool update_image;

    // you must insert your service key here!    
    private string authorizationKey = "ab5992b690a447ec84d4b4d40138dfc2";
    private const string ocpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
    private string visionAnalysisEndpoint = "https://eastus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Tags";   // This is where you need to update your endpoint, if you set your location to something other than west-us.

    internal byte[] imageBytes;

    internal string imagePath;
    internal string textPath;

    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;

        textPath = Path.Combine(Application.persistentDataPath, "img_str.txt");

        update_image = false;
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

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post("http://localhost:5000/endpoint", webForm))
        {
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                print(unityWebRequest.error);
            }
            else
            {
                print("Finished Uploading Screenshot");
                String jsonresponse = unityWebRequest.downloadHandler.text;

                // Debug.Log(jsonresponse);

                AnalysedObject analysedObject = new AnalysedObject();
                analysedObject = JsonUtility.FromJson<AnalysedObject>(jsonresponse);

                String img_str = analysedObject.img_str;

                byte[] img_bytes = System.Text.Encoding.ASCII.GetBytes(img_str);

                Debug.Log(Application.dataPath);
                //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.jpg", img_bytes);
                

          

                //byte[] b = tex.EncodeToPNG();
                //UnityEngine.Object.Destroy(tex);
                //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", b);



                String utf_string = System.Text.Encoding.UTF8.GetString(img_bytes);

                //Write some text to the test.txt file
                StreamWriter writer = new StreamWriter(textPath, true, System.Text.Encoding.UTF8);
                writer.WriteLine(utf_string);
                writer.Close();

                // Convert Base64 Encoded string to Byte Array.
                byte[] imageBytes = Convert.FromBase64String(utf_string);
                File.WriteAllBytes(Application.dataPath + "/../SavedScreen.jpg", imageBytes);

                // create a texture that can be drawn to the screen
                tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);


                GameObject go = GameObject.Find("RawImage");
                UnityEngine.UI.RawImage m_RawImage = go.GetComponent<UnityEngine.UI.RawImage>();
                m_RawImage.texture = tex;

                //update_image = true;

            }
        }

        //using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(visionAnalysisEndpoint, webForm))
        //{
        //    // gets a byte array out of the saved image
        //    imageBytes = GetImageAsByteArray(imagePath);
        //    unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
        //    unityWebRequest.SetRequestHeader(ocpApimSubscriptionKeyHeader, authorizationKey);

        //    String s = Convert.ToBase64String(imageBytes);

        //    // the download handler will help receiving the analysis from Azure
        //    unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

        //    // the upload handler will help uploading the byte array with the request
        //    unityWebRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
        //    unityWebRequest.uploadHandler.contentType = "application/octet-stream";

        //    Debug.Log(s);

        //    yield return unityWebRequest.SendWebRequest();

        //    long responseCode = unityWebRequest.responseCode;

        //    try
        //    {
        //        string jsonResponse = null;
        //        jsonResponse = unityWebRequest.downloadHandler.text;

        //        // The response will be in Json format
        //        // therefore it needs to be deserialized into the classes AnalysedObject and TagData
        //        AnalysedObject analysedObject = new AnalysedObject();
        //        analysedObject = JsonUtility.FromJson<AnalysedObject>(jsonResponse);

        //        if (analysedObject.tags == null)
        //        {
        //            Debug.Log("analysedObject.tagData is null");
        //        }
        //        else
        //        {
        //            Dictionary<string, float> tagsDictionary = new Dictionary<string, float>();

        //            foreach (TagData td in analysedObject.tags)
        //            {
        //                TagData tag = td as TagData;
        //                tagsDictionary.Add(tag.name, tag.confidence);
        //            }

        //            ResultsLabel.instance.SetTagsToLastLabel(tagsDictionary);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        Debug.Log("Json exception.Message: " + exception.Message);
        //    }

        //    yield return null;
        //}
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
