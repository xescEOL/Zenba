using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateQrScript : MonoBehaviour
{
    public RawImage cRawImage;
    void Start()
    {
        Texture2D img = QR.GenerateBarcode("hello world!", (int)cRawImage.rectTransform.rect.width, (int)cRawImage.rectTransform.rect.width);
        cRawImage.texture = img;
        cRawImage.rectTransform.sizeDelta = new Vector2(img.width, img.height);

    }

    // Update is called once per frame
    void Update()
    {

    }


}
