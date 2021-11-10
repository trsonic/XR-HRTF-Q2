using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerAnchorPositioning : MonoBehaviour
{
    #region Public Variables
    public GameObject _speakerAnchor;
    #endregion

    #region Private Variables

    OVRInput.Controller controller = OVRInput.Controller.RTouch;

    private bool _anchorPositioning = true;
    #endregion

    void Start()
    {
        TextDisplay.Instance.ShowMainTextDisplay(true);
    }
    void OnDestroy()
    {

    }
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, controller))
        {
            _anchorPositioning = !_anchorPositioning;
            TextDisplay.Instance.ShowMainTextDisplay(_anchorPositioning);
            ShowSpeakerAnchor(_anchorPositioning);
        }

        if(_anchorPositioning)
        {
            // check if speaker anchor is visible

            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
            {
                Vector3 speakerOrientation = new Vector3(0.0f, OVRInput.GetLocalControllerRotation(controller).eulerAngles.y, 0.0f);
                _speakerAnchor.transform.eulerAngles = speakerOrientation;
                _speakerAnchor.transform.position = OVRInput.GetLocalControllerPosition(controller);
                _speakerAnchor.transform.Translate(0.0f, 0.0f, 0.1f);
            }
        }
    }
    void ShowSpeakerAnchor(bool show)
    {
        int childCount = _speakerAnchor.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            _speakerAnchor.transform.GetChild(i).GetComponent<Renderer>().enabled = show;
        } 
    }
}
