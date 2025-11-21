Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _RendererColor;
            fixed4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.rgb *= c.a;

                // Sample neighboring pixels for outline detection
                float outline = 0.0;
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;

                // Check 8 directions around current pixel
                outline += tex2D(_MainTex, IN.texcoord + float2(texelSize.x, 0)).a;
                outline += tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, 0)).a;
                outline += tex2D(_MainTex, IN.texcoord + float2(0, texelSize.y)).a;
                outline += tex2D(_MainTex, IN.texcoord + float2(0, -texelSize.y)).a;
                outline += tex2D(_MainTex, IN.texcoord + float2(texelSize.x, texelSize.y)).a;
                outline += tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, texelSize.y)).a;
                outline += tex2D(_MainTex, IN.texcoord + float2(texelSize.x, -texelSize.y)).a;
                outline += tex2D(_MainTex, IN.texcoord + float2(-texelSize.x, -texelSize.y)).a;

                // If current pixel is transparent but neighbors are opaque, draw outline
                if (c.a < 0.01 && outline > 0.01)
                {
                    c = _OutlineColor;
                    c.rgb *= c.a;
                }

                return c;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
