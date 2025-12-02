Shader "Custom/MechSilhouetteOutline"
{
    Properties
    {
        _MainTex ("Mech Composite", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(1, 10)) = 4
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent-1"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "OutlinePass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // If current pixel is opaque (part of sprite), return transparent
                // The sprite itself is rendered separately by the main camera
                if (mainColor.a > 0.1)
                {
                    return half4(0, 0, 0, 0);
                }

                // Sample in circle pattern for smooth outline
                float maxAlpha = 0.0;
                int samples = 16;

                for (int i = 0; i < samples; i++)
                {
                    float angle = (float(i) / float(samples)) * 6.28318530718; // 2 * PI
                    float2 offset = float2(cos(angle), sin(angle)) * _OutlineWidth;
                    float2 sampleUV = uv + offset * _MainTex_TexelSize.xy;

                    // Clamp to valid UV range
                    sampleUV = clamp(sampleUV, 0.0, 1.0);

                    half sampleAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV).a;
                    maxAlpha = max(maxAlpha, sampleAlpha);
                }

                // If any neighbor is opaque, draw outline
                if (maxAlpha > 0.1)
                {
                    return half4(_OutlineColor.rgb, _OutlineColor.a);
                }

                return half4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
