using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{

    [SerializeField]
    Transform pointPrefab;

    // Amount of cubes in the scene
    [SerializeField, Range(10, 100)]
    int resolution = 10;

     [SerializeField]
    FunctionLibrary.FunctionName function;

    // Array to keep the reference of each point in the scene
    Transform[] points;


    void Awake()
    {   
        // Configure an offset so to make sure that all the points stay at a -1/1 range
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;

        points = new Transform[resolution * resolution];

        for (int i = 0; i < points.Length; i++)
        {

            Transform point = Instantiate(pointPrefab);
            // Make a reference to each point we have
            points[i] = point;
        
            point.localScale = scale;

            // Instatiate them all inside the graphs gameObject
            point.SetParent(transform, false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);

        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f; // Gets v initial position

        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {

            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f; // Recalculate v when z changes
            }

            float u = (x + 0.5f) * step - 1f;
            // Dynamically assign the points coordinates based on the function over time
            points[i].localPosition = f(u, v, time);
        }   
    }
}
