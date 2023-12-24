using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TesseractDriver
{
    private TesseractWrapper _tesseract;
    private static readonly List<string> fileNames = new List<string> {"tessdata.tgz"};

    public string CheckTessVersion()
    {
        _tesseract = new TesseractWrapper();

        try
        {
            string version = "Tesseract version: " + _tesseract.Version();
            Debug.Log(version);
            return version;
        }
        catch (Exception e)
        {
            string errorMessage = e.GetType() + " - " + e.Message;
            Debug.LogError("Tesseract version: " + errorMessage);
            return errorMessage;
        }
    }

    public void Setup(UnityAction onSetupComplete)
    {
#if UNITY_EDITOR
        OcrSetup(onSetupComplete);
#elif UNITY_ANDROID
        CopyAllFilesToPersistentData(fileNames, onSetupComplete);
#else
        OcrSetup(onSetupComplete);
#endif
    }

    public void OcrSetup(UnityAction onSetupComplete)
    {
        _tesseract = new TesseractWrapper();

#if UNITY_EDITOR
        string datapath = Path.Combine(Application.streamingAssetsPath, "tessdata");
#elif UNITY_ANDROID
        string datapath = Application.persistentDataPath + "/tessdata/";
#else
        string datapath = Path.Combine(Application.streamingAssetsPath, "tessdata");
#endif

        if (_tesseract.Init("rus", datapath))
        {
            Debug.Log("Init Successful");
            onSetupComplete?.Invoke();
        }
        else
        {
            Debug.LogError(_tesseract.GetErrorMessage());
        }
    }

    private async void CopyAllFilesToPersistentData(List<string> fileNames, UnityAction onSetupComplete)
    {
        String fromPath = "jar:file://" + Application.dataPath + "!/assets/";
        String toPath = Application.persistentDataPath + "/";

        foreach (String fileName in fileNames)
        {
            if (!File.Exists(toPath + fileName))
            {
                Debug.Log("Copying from " + fromPath + fileName + " to " + toPath);
                WWW www = new WWW(fromPath + fileName);

                while (!www.isDone)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
                }

                var wwwBytes = www.bytes;
                await File.WriteAllBytesAsync(toPath + fileName, wwwBytes);
                Debug.Log("File copy done");
                www.Dispose();
                www = null;
            }
            else
            {
                Debug.Log("File exists! " + toPath + fileName);
            }

            UnZipData(fileName);
        }

        OcrSetup(onSetupComplete);
    }

    public string GetErrorMessage()
    {
        return _tesseract?.GetErrorMessage();
    }

    public string Recognize(Texture imageToRecognize, bool isDraw = false)
    {
        if (imageToRecognize is Texture2D texture2D)
            return _tesseract.Recognize(texture2D, isDraw);
        if (imageToRecognize is RenderTexture renderTexture)
            return _tesseract.Recognize(renderTexture, isDraw);
        return null;
    }

    public Texture GetHighlightedTexture()
    {
        return _tesseract.GetHighlightedTexture();
    }

    public Texture2D CollectBiggestBoxPattern()
    {
        return _tesseract.CollectMaxRect();
    }

    private void UnZipData(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            UnZipUtil.ExtractTGZ(Application.persistentDataPath + "/" + fileName, Application.persistentDataPath);
            Debug.Log("UnZipping Done");
        }
        else
        {
            Debug.LogError(fileName + " not found!");
        }
    }
}