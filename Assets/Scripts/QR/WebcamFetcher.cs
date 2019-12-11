using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WebcamFetcher : MonoBehaviour
{
    private RenderTexture _vertical;
    private RenderTexture _horizontal;
    private Camera _camera;

    // Update is called once per frame
    public RenderTexture RenderTexture
    {
        get
        {
            var orientation = Screen.orientation;
            if (orientation == ScreenOrientation.Landscape || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
            {
                return _horizontal;
            }
            else
            {
                return _vertical;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        _camera = GetComponent<Camera>();
        _horizontal = new RenderTexture(Screen.width, Screen.height, 24);
        _vertical = new RenderTexture(Screen.height, Screen.width, 24);
    }

    // Update is called once per frame
    void Update()
    {
        var orientation = Screen.orientation;
        if (orientation == ScreenOrientation.Landscape || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight)
        {
            _camera.targetTexture = _horizontal;
        }
        else
        {
            _camera.targetTexture = _vertical;
        }
    }
}