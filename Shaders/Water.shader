Shader "Unlit/Water"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ReflectionTexture ("Texture", 2D) = "white" {}
		_RefractionTexture ("Texture", 2D) = "white" {}
	}
	SubShader
	{
        Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 refl : TEXCOORD1;
				float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float height : FLOAT;
                float3 worldToCam : COLOR1;


			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _ReflectionTexture;
			sampler2D _RefractionTexture;
			
			v2f vert (appdata v)
			{
				v2f o;
                o.height = 0.5;
                o.height += cos(v.vertex.x    + _Time.z) * 0.05;
                o.height += cos(v.vertex.x/10 + _Time.y) * 0.2;
                o.height += sin(v.vertex.z/5  + _Time.x) * 0.1;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.refl = ComputeNonStereoScreenPos(o.vertex);
                o.worldToCam = WorldSpaceViewDir(v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// Reflection
                float height = (i.height * 2 - 1)*0.02;
                float frenselFactor = dot(normalize(i.worldToCam), float3(0, 1, 0));

                float4 reflUv = i.refl / i.refl.w;
                float4 refrUv = reflUv;

                reflUv.y = 1 - reflUv.y + height;
                reflUv.x = reflUv.x + height;

                refrUv.y = refrUv.y + height;
                refrUv.x = refrUv.x + height;

                float4 reflectionCol = tex2Dproj(_ReflectionTexture, UNITY_PROJ_COORD(reflUv));     
                float4 refractionCol = tex2Dproj(_RefractionTexture, UNITY_PROJ_COORD(refrUv));
            
                float4 col = lerp(reflectionCol, refractionCol, frenselFactor);
            
                return lerp(col, float4(0.1, 0.5, 1, 1), 0.1);
			}
			ENDCG
		}
	}
}
