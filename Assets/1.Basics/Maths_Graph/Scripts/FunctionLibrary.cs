using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.Mathf;

/*
LIBRARY THAT STORES DIFFERENT FUNCTIONS THAT RETURNS
PARAMETRIC SURFACES BY GIVING ANGULAR COORDINATES θ (v), φ (u) ∈ [0, 2π),
*/

public static class FunctionLibrary
{


    // DELEGATE : Is a "pointer" function that represents
    // a specific functions from our class
    public delegate Vector3 Function(float u, float v, float t);

    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus }

    // Array that store the math functions
    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

    // Method to choose a math function based on index
    
    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    // SEQUENCE
    // Pass to the next function or return to the initial one
    public static FunctionName GetNextFunctionName(FunctionName name)
    {
        if ((int)name < functions.Length - 1)
        {
            return name + 1;
        }
        else
        {
            return 0;
        }
    }

    // RANDOM
    // Get a random function number besides the one as a parameter
    public static FunctionName GetRandomFunctionNameOtherThan(FunctionName name)
    {
        FunctionName choice = (FunctionName)Random.Range(1, functions.Length);
        return choice == name ? 0 : choice;
    }


    public static Vector3 Morph(
        float u, float v, float t, Function from, Function to, float progress
    )
    {
        // Linear interpolation between the functions. It will prodice a straight constant-speed
        // transition between the functions.
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
    }


    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;

    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t)); // Wave 1 slowed by half
        p.y += 0.5f * Sin(2f * PI * (v + t)); // Wave 2 with amplitude 2u 
        p.y += Sin(PI * (u + v + 0.25f * t)); // Wave 3 slowed by a quarter
        // Since the max and min values of this function could be 2.5 and -2.5
        // Divide by 2.5 to garantee the -1/1 range
        p.y *= (1f / 2.5f);
        p.z = v;
        return p;
    }

    // RIPPLE FUNCTION
    // Reduces the amplitude based on the absolute distance from the origin
    public static Vector3 Ripple(float u, float v, float t)
    {
        float d = Sqrt(u * u + v * v); // 2D distance from the origin (Pythagorean theorem)
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= (1f + 10f * d);
        p.z = v;
        return p;
    }

    // SHEPRE FUNCTION
    // Creates a sphere that expands and contracts its radius 
    public static Vector3 Sphere(float u, float v, float t)
    {
        // Radius
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t)); // Makes the radius be between 0 and 1/2
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;

        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t)); // Outer ring 
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));// Inner ring (thickness)
        float s = r1 + r2 * Sin(PI * v); // Expand the minimum circles so they can have a 0.5 radius
        Vector3 p;

        p.x = s * Cos(PI * u);
        p.y = s * Sin(PI * u);
        p.z = r2 * Cos(PI * v);

        return p;
    }
}
