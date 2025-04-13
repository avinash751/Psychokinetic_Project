Shader "UI/Unlit/FourCornerGradient"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        [HDR]_TopLeftColor ("Top Left Color", Color) = (1,0,0,1)
        [HDR]_TopRightColor ("Top Right Color", Color) = (0,1,0,1)
        [HDR]_BottomLeftColor ("Bottom Left Color", Color) = (0,0,1,1)
        [HDR]_BottomRightColor ("Bottom Right Color", Color) = (1,1,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Stencil
        {
            Ref 1
            Comp Greater
            Pass Replace
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _TopLeftColor;
            fixed4 _TopRightColor;
            fixed4 _BottomLeftColor;
            fixed4 _BottomRightColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 colorTop = lerp(_TopLeftColor, _TopRightColor, i.uv.x);
                fixed4 colorBottom = lerp(_BottomLeftColor, _BottomRightColor, i.uv.x);
                return lerp(colorTop, colorBottom, i.uv.y); 
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}