using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class Movement : MonoBehaviour
{
    [System.Serializable]
    public class DataEntry
    {
        public int t;
        public int p0;
        public int p1;
    }

    public List<DataEntry> stepsList = new List<DataEntry>();

    public int currentStep = 0; // Counter for steps

    // The URL of the file to download
    public string fileUrl = "http://127.0.0.1:5000/download-csv";
    public string savePath = "Assets/Resources/steps.csv";

    void Update()
    {
        Debug.Log("here");
        if (Input.GetKeyDown(KeyCode.RightArrow) && currentStep < stepsList.Count - 1)
        {
            currentStep++;
            Debug.Log(currentStep);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentStep > 0)
        {
            currentStep--;
            Debug.Log(currentStep);
        }
        Debug.Log("here");
        MoveToStep();
    }

    void Start()
    {
        // Start the coroutine to download the CSV file
        StartCoroutine(DownloadAndReadFile());
    }

    void MoveToStep()
    {
        // Adjust the target position based on the CSV data
        if (stepsList.Count == 0) return;
        Vector3 targetPosition = new Vector3(stepsList[currentStep].p1 * 10, 2.5f, stepsList[currentStep].p0 * 10);
        transform.position = targetPosition;
    }

    void ReadCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources: " + fileName);
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 0; i < lines.Length; i++) 
        {
            string[] values = lines[i].Split(',');
            if (values.Length < 3) continue;

            DataEntry entry = new DataEntry
            {
                t = int.Parse(values[1].Trim()),
                p1 = int.Parse(values[2].Trim()) - 15,
                p0 = int.Parse(values[3].Trim()) - 15
            };
            stepsList.Add(entry);
        }
    }

    void PrintData()
    {
        foreach (var step in stepsList)
        {
            Debug.Log($"t: {step.t}, p0: {step.p0}, p1: {step.p1}");
        }
    }

    IEnumerator DownloadAndReadFile()
    {
        // Create a UnityWebRequest to fetch the file
        UnityWebRequest request = UnityWebRequest.Get(fileUrl);

        // Send the request and wait for it to complete
        yield return request.SendWebRequest();

        // Check if there was an error in the request
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download file: " + request.error);
        }
        else
        {
            // Successfully downloaded the file, now save it
            System.IO.File.WriteAllBytes(savePath, request.downloadHandler.data);
            Debug.Log("File successfully downloaded and saved at: " + savePath);

            // Now read the CSV file
            ReadCSV("steps");
            PrintData();
        }
        Debug.Log("Coroutine finished.");
    }
}