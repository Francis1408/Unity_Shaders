using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*

CLASS THAT MANAGES THE FUNCTION ASSIGMENT. IN THE INSPECTOR YOU CAN CHOOSE THE FOLLOWING OPTIONS:

- Resolution : How much cubes (pointPrefab) will be rendered 
- Transition Mode: Can be either RANDOM or SEQUENTIAL
- Transition Duration: How long does it take to transition between functions
(The transition function uses liner interpolation for a smoother transition)
- Function Duration: How long the function stays rendered on screen

*/


public class Graph : MonoBehaviour
{

    [SerializeField]
    Transform pointPrefab;

    // Amount of cubes in the scene
    [SerializeField, Range(10, 100)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    // Choose between random o sequence change
    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;


    // Array to keep the reference of each point in the scene
    Transform[] points;

    // How much it keeps on each function before switching
    float duration;

    // Flag to turn ON/OFF the transition mode
    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;


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

    // Take the following function to be rendered based on the mode settled
    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
        FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }


    // Update is called once per frame
    void Update()
    {
        // Keeps track on time given to each function
        duration += Time.deltaTime;
        // If we are transitioning, the transitionDuration should not count
        // in the time that the function will be on the screen
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }

        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            // Assign the next function
            PickNextFunction();
        }

        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }

    }

    // Normal function update (withou transitioning)
    void UpdateFunction()
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

    // Updates the functions with transition enabled
    void UpdateFunctionTransition()
    {
        FunctionLibrary.Function
            from = FunctionLibrary.GetFunction(transitionFunction),
            to = FunctionLibrary.GetFunction(function);

        float progress = duration / transitionDuration;
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
            points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
    }
}
