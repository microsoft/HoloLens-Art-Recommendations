using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;

public class ImageCapture : MonoBehaviour {

    public static ImageCapture instance;
    private PhotoCapture photoCaptureObject = null;
    private GestureRecognizer recognizer;
    private bool currentlyCapturing = false;

    private void Awake()
    {
        // Allows this instance to behave like a singleton
        instance = this;

    }

    void Start()
    {
        // subscribing to the Hololens API gesture recognizer to track user gestures
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.Tapped += TapHandler;
        recognizer.StartCapturingGestures();
    }

    void Update()
    {
        // space bar manually triggers a picture event
        // this is used for running in unity
        if (Input.GetKeyDown("space"))
        {
            print("space key was pressed");

            // Only allow capturing, if not currently processing a request.
            if (currentlyCapturing == false)
            {
                currentlyCapturing = true;

                // Create a label in world space using the ResultsLabel class
                ResultsLabel.instance.CreateLabel();

                // Begins the image capture and analysis procedure
                ExecuteImageCaptureAndAnalysis();
            }
        }
    }

    /// <summary>
    /// Respond to Tap Input.
    /// </summary>
    private void TapHandler(TappedEventArgs obj)
    {
        // Only allow capturing, if not currently processing a request.
        if (currentlyCapturing == false)
        {
            currentlyCapturing = true;

            // Create a label in world space using the ResultsLabel class
            ResultsLabel.instance.CreateLabel();

            // Begins the image capture and analysis procedure
            ExecuteImageCaptureAndAnalysis();
        }
    }

    /// <summary>
    /// Register the full execution of the Photo Capture. If successful, it will begin 
    /// the Image Analysis process.
    /// </summary>
    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        // Call StopPhotoMode once the image has successfully captured
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);

        Debug.Log("on captured photo to disk");
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Dispose from the object in memory and request the image analysis 
        // to the VisionManager class
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
        StartCoroutine(VisionManager.instance.AnalyseLastImageCaptured());
    }

    /// <summary>    
    /// Begin process of Image Capturing and send To Azure     
    /// Computer Vision service.   
    /// </summary>    
    private void ExecuteImageCaptureAndAnalysis()
    {
        // Set the camera resolution to be the highest possible    
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        Debug.Log("execute image capture and analysis");

        // Begin capture process, set the image format    
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            CameraParameters camParameters = new CameraParameters();
            camParameters.hologramOpacity = 0.0f;
            camParameters.cameraResolutionWidth = targetTexture.width;
            camParameters.cameraResolutionHeight = targetTexture.height;
            camParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Capture the image from the camera and save it in the App internal folder    
            captureObject.StartPhotoModeAsync(camParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                string filename = @"current_image.jpg";

                Debug.Log(Application.persistentDataPath);

                string filePath = Path.Combine(Application.persistentDataPath, filename);

                VisionManager.instance.imagePath = filePath;
                
                photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);

                currentlyCapturing = false;
            });
        });
    }

}
