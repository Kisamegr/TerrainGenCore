Shader "Custom/TerrainSurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

        const static int maxLayers = 10;
        const static float epsilon = 1E-4;

        int layers;
        float minHeight;
        float maxHeight;

        float startHeights[maxLayers];
        float blendHeights[maxLayers];
        float3 tintColors[maxLayers];
        float tintColorBlends[maxLayers];
        float textureScales[maxLayers];

        UNITY_DECLARE_TEX2DARRAY(albedoTextures);
        UNITY_DECLARE_TEX2DARRAY(normalTextures);

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

            

        float inverseLerp(float min, float max, float value) {
            return saturate((value - min) / (max - min));
        }

        float3 albidoTriplanar(float3 worldPos, float3 blendAxis, int textureIndex) {
            float3 scaledWorldPos = worldPos / textureScales[textureIndex];
            fixed4 xProjection = UNITY_SAMPLE_TEX2DARRAY(albedoTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxis.x;
            fixed4 yProjection = UNITY_SAMPLE_TEX2DARRAY(albedoTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxis.y;
            fixed4 zProjection = UNITY_SAMPLE_TEX2DARRAY(albedoTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxis.z;

            return xProjection + yProjection + zProjection;
        } 
        
        float3 normalTriplanar(float3 worldPos, float3 blendAxis, int textureIndex) {
            float3 scaledWorldPos = worldPos / textureScales[textureIndex];
            fixed4 xProjection = UNITY_SAMPLE_TEX2DARRAY(normalTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxis.x;
            fixed4 yProjection = UNITY_SAMPLE_TEX2DARRAY(normalTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxis.y;
            fixed4 zProjection = UNITY_SAMPLE_TEX2DARRAY(normalTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxis.z;

            xProjection.xy = (xProjection.wy * 2 - 1);
            yProjection.xy = (yProjection.wy * 2 - 1);
            zProjection.xy = (zProjection.wy * 2 - 1);

            return xProjection + yProjection + zProjection;
        }


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
            float height = inverseLerp(minHeight, maxHeight, IN.worldPos.y);

            float3 normalBlend = abs(IN.worldNormal);
            normalBlend /= normalBlend.x + normalBlend.y + normalBlend.z;

            half3 triplanarBlend = saturate(pow(IN.worldNormal, 4));
            triplanarBlend /= max(dot(triplanarBlend, half3(1, 1, 1)), 0.0001);


            for (int i = 0; i < layers; i++) {
                float drawStrength = inverseLerp(-blendHeights[i]/2-epsilon, blendHeights[i]/2, height - startHeights[i]);

                float3 textureColor = albidoTriplanar(IN.worldPos, triplanarBlend, i);
                float3 color = (1 - tintColorBlends[i]) * textureColor + tintColorBlends[i] * tintColors[i];

                float3 normal = normalTriplanar(IN.worldPos, triplanarBlend, i);

                o.Albedo = o.Albedo * (1 - drawStrength) + drawStrength * color;
                //o.Normal = o.Normal * (1 - drawStrength) + drawStrength * normal;
            }

			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex / textureScales[0]) * _Color;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
