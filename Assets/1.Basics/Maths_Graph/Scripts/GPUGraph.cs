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


The class instructs to the GPU to run a compute shader kernel and then tells Unity to procedually draw a
lot of points.
*/


public class GPUGraph : MonoBehaviour
{

    // Amount of cubes in the scene
    [SerializeField, Range(10, 1000)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    // Choose between random o sequence change
    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    //  ###### SHADER ASSETS ##############################
    [SerializeField]
    Material material;
    // What we'll instruct the GPU to draw a specific mesh with a specific material many times, 
    // via a single command
    [SerializeField]
    Mesh mesh;

    // Get the reference from out computeShader
    [SerializeField]
    ComputeShader computeShader;

    // Buffer that will pass info for the GPU
    ComputeBuffer positionBuffer;

    // The ID's declarations which will be used for the GPU calculation
    // The int is just a reference/handle to a uniform variable slot in the compiled shader program.
    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    // Keeps the kernelId
    int kernelId;
    //  ####################################################

    // How much it keeps on each function before switching
    float duration;

    // Flag to turn ON/OFF the transition mode
    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    void OnEnable()
    {
        // Argument 1: Buffer size (the amount of points)
        // Argument 2: The size of each element (3D float coordinates = 3 * 4 bytes)
        positionBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }

    void OnDisable()
    {
        // Free buffer memory
        positionBuffer.Release();
        // This makes it possible for the object to be reclaimed by Unity's memory garbage collection 
        // process the next time it runs
        positionBuffer = null;
    }

    // Update the values from the ID's (Uniforms) 
    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);

        // if we're transitioning, otherwise don't bother.
        if (transitioning)
        {
            computeShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }

        // Get the random num of the function that will be assigned
        // Plus the funtion that will be transitioning
        int kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function) * 5;

        // Assign the buffer to the kernel
        // Argument 1 : Which kernel the buffer must be assigned
        computeShader.SetBuffer(kernelIndex, positionsId, positionBuffer);

        // Decide how many groups are nedded to draw
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        // Pass the values to the material shader
        material.SetBuffer(positionsId, positionBuffer);
        material.SetFloat(stepId, step);

        //Define the Frustum = The area bound where the mesh will be drawn
        // If the position of the mesh ends up to be outside the frustum, 
        // so it is discarted
        var bounds = new Bounds(Vector3.zero, Vector3.one *  (2f + 2f / resolution));

        // Procedural Drawing: Calls the GPU to draw the mesh
        // Must to know what to draw (mesh, material)
        // Where to draw (bounds)
        // How many to draw (positionBuffer size)
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionBuffer.count);
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

        // Assign the uniforms and dispatch it to the GPU
        UpdateFunctionOnGPU();

    }
    
}
