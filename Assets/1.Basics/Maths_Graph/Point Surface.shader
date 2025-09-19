Shader "Custom/Point Surface"
{
    // Like a SerializeField that lets the user change the shader properties
    Properties
    {
        _Smoothness ("Smoothness", Range(0,1)) = 0.5 // Label and Range
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface ConfigureSurface Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        // Sets a minimum for the shader's target level and quality
        #pragma target 3.0

        struct Input {

            float3 worldPos : WORLDPOS; // float3 is the same as a Vector3 (float, float, float)
        };

        float _Smoothness;
        
        // First is an input parameter that has the Input type that we just defined. 
        // The second parameter is the surface configuration data, with the type SurfaceOutputStandard.
        // The second parameter must have the inout keyword, which indicates that it's both passed to the function and used for the result of the function. 
        void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
            
            // Sets the (R, G) as (X, Y) coordinates of the cube
            surface.Albedo = saturate(input.worldPos * 0.5 + 0.5); // saturate clamps all values to 0-1 
            surface.Smoothness = _Smoothness;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
