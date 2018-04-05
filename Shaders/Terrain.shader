Shader "Unlit/Terrain"
{
	Properties
	{
		_MainTex  ("Texture", 2D) = "white" {}
		_ColorTex ("Texture", 2D) = "white" {}
        _HeightScale("HeightScale", Float) = 1
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
                float height : FLOAT;
			};

			sampler2D _MainTex;
			sampler2D _ColorTex;
			float4 _MainTex_ST;
            float _HeightScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.height = v.vertex.y/ _HeightScale;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);


                float2 s = float2(i.uv.x, i.height);
				fixed4 col = tex2D(_ColorTex, s);

				return col;
			}
			ENDCG
		}
	}
}
