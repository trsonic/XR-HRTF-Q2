using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour
{

    #region Singleton
    private static TextDisplay _instance;

    public static TextDisplay Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TextDisplay();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public GameObject speakerAnchor, mainCamera;
    private GameObject mainDisplay, hmdDisplay;

    void Start()
    {
        mainDisplay = transform.Find("MainDisplay").gameObject;
        hmdDisplay = transform.Find("HmdDisplay").gameObject;

        ShowHmdTextDisplay(false);
    }

    void Update()
    {
        mainDisplay.transform.rotation = speakerAnchor.transform.rotation;
        mainDisplay.transform.position = speakerAnchor.transform.position;
        mainDisplay.transform.Translate(0.0f, 0.3f, 0.0f);

        hmdDisplay.transform.position = mainCamera.transform.position;
        hmdDisplay.transform.rotation = mainCamera.transform.rotation;
        hmdDisplay.transform.Translate(new Vector3(0.0f, 0.0f, 1.0f));
    }

    public void PrintMessage(string message)
    {
        mainDisplay.GetComponent<TextMesh>().text = message + "\n";
    }

    public void PrintMessageHMD(string message)
    {
        hmdDisplay.GetComponent<TextMesh>().text = message + "\n";
    }

    public void ShowMainTextDisplay(bool show)
    {
        mainDisplay.GetComponent<MeshRenderer>().enabled = show;
    }

    public void ShowHmdTextDisplay(bool show)
    {
        hmdDisplay.GetComponent<MeshRenderer>().enabled = show;
    }
}
