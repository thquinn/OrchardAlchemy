Shader "thquinn/Grass"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SparsenessTex ("Sparseness (RGB)", 2D) = "white" {}
        _WindTex ("Wind (RGB)", 2D) = "white" {}
        _GridTex ("Grid (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 worldSpacePos : TEXCOORD3;
            };

            sampler2D _MainTex, _SparsenessTex, _WindTex, _GridTex;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float2 rotateUV(float2 uv, float rotation)
            {
                float mid = 0.5;
                return float2(
                    cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
                    cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
                );
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float2 windUV = i.worldSpacePos * .005;
                windUV.x += _Time.y * .002;
                windUV.y -= _Time.y * .005;
                float wind = tex2D(_WindTex, windUV);
                wind = lerp(.4, .6, wind);
                float2 mainUV = i.worldSpacePos * .441;
                mainUV.xy += wind * .2;
                float main = tex2D(_MainTex, mainUV);
                main = lerp(.8, 1.2, main);
                float glint = pow(tex2D(_WindTex, windUV) * main, 10);
                main += glint * .2;
                float grid = tex2D(_GridTex, i.worldSpacePos);
                grid = lerp(.8, 1, grid);
                return main * _Color * grid;
            }
            ENDCG
        }
    }
}
