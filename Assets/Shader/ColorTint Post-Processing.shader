Shader "Custom Post-Processing/ColorTint Post-Processing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_blend("B & W blend", Range(0,1)) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            uniform sampler2D _MainTex;
            float _Intensity;
            float4 _OverlayColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 result = tex2D(_MainTex, i.uv) * _OverlayColor;
                result.rgb *= _Intensity;
                return result;

                // float4 c = tex2D(_MainTex, i.uv);
                // // calculate luminance
                // float lum = c.r * .3 + c.g * .59 + c.b * .11;
                // // create a black and white color
                // float3 bwc = float3(lum, lum, lum);
    
                // float4 result = c;
                // result.rgb = lerp(c, bwc, _blend);
                // return result;
            }
            ENDHLSL
        }
    }
}
