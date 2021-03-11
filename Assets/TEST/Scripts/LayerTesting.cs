using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class LayerTesting : MonoBehaviour
{
    LayersController controller;
    public Texture tex;

    public float speed;
    public bool smooth;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = LayersController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        LayersController.LAYER layer = null;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            layer = controller.background;
            layer.SetTexture(tex);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            layer = controller.foreground;
            layer.SetTexture(tex);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            layer = controller.background;
            layer.TransitionToTexture(tex, speed, smooth);
        }

    }
}
