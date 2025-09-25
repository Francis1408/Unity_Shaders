Shader "Custom/Point Surface GPU"
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
        #pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        // Indicates the surface shader needs to invoke a ConfigureProcedural
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural

        // Use shader model 3.0 target, to get nicer looking lighting
        // Sets a minimum for the shader's target level and quality
        #pragma target 4.5

        struct Input {

            float3 worldPos; // float3 is the same as a Vector3 (float, float, float)
        };

        float _Smoothness;
        float _Step;

        // Gets the reference from the buffer
        // It is wrapped in a conditinal that tells the compiler only to include this code part
        // if the label is defined
        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<float3> _Positions;
        #endif
        
        void ConfigureProcedural () {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                float3 position = _Positions[unity_InstanceID];

                // Set the entire transformation matrix to zero
                unity_ObjectToWorld = 0.0;
                // Fill the values of the matrix with the cube positions
                /*
                    |s.x 0   0   p.x|
                    |0  s.y  0   p.y|
                    |0   0  s.z   0 |
                    |0   0   0    1 |
                */  
                unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
                // Scale
                unity_ObjectToWorld._m00_m11_m22 = _Step;
            #endif
        }

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
