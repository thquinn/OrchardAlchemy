Shader "thquinn/TickDisc"
{
	Properties{
		_Color ("Tint", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_Revealed ("Revealed", Range(0, 1)) = 0
	}

		SubShader{
		Tags{ 
				"RenderType"="Transparent" 
				"Queue"="Transparent"
	}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off

		Pass{

				CGPROGRAM

#include "UnityCG.cginc"

#pragma vertex vert
#pragma fragment frag

				sampler2D _MainTex;
	float4 _MainTex_ST;

	fixed4 _Color;
	float _Revealed;

	struct appdata{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct v2f{
		float4 position : SV_POSITION;
		float2 uv : TEXCOORD0;
		fixed4 color : COLOR;
	};

	v2f vert(appdata v){
		v2f o;
		o.position = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : SV_TARGET{
		fixed4 col = tex2D(_MainTex, i.uv);
		col *= _Color;
		col *= i.color;
		float theta = atan2(.5 - i.uv.x, .5 - i.uv.y) + 3.141;
		col.a *= smoothstep(_Revealed + .01, _Revealed, theta / 6.28);
		return col;
	}

		ENDCG
	}
	}
}