Shader "Unlit/TextShader"
{
	Properties
	{
		//Basic
		_BasicColAndRoughnessTex("Basic Color & Roughness Texture", 2D) = "white"{}
		_InteriorColAndSpecularTex("Interior Color & Specular Texture", 2D) = "white"{}
		_Tint("Tint", Color) = (1, 1, 1, 1)

		// specular
		_ScpecularRate("Specular Rate", Range(1, 5)) = 1
		_NoiseTex("Noise Texture", 2D) = "White" {}

		// Directional Subsurface Scattering
		_FrontSssDistortion("Front SSS Distortion", Range(0, 1)) = 0.5
		_BackSssDistortion("Back SSS Distortion", Range(0, 1)) = 0.5
		_FrontSssIntensity("Front SSS Intensity", Range(0, 1)) = 0.2
		_BackSssIntensity("Back SSS Intensity", Range(0, 1)) = 0.2
		_InteriorColorPower("Interior Color Power", Range(0, 5)) = 2

		// Lighting
		_UnlitRate("Unlit Rate", range(0, 1)) = 0.5
		_AmbientLight("Ambient Light", Color) = (0.5, 0.5, 0.5, 1)

		// fresnel
		_FresnelPower("Fresnel Power", Range(0.0, 36)) = 0.1
		_FresnelIntensity("Fresnel Intensity", Range(0, 1)) = 0.2

		// side rim
		_RimColor("Rim Color", Color) = (0.5, 0.5, 0.5, 1)
		_RimIntensity("Rim Intensity", Range(0.0, 5)) = 1.0
		_RimLightSampler("Rim Mask", 2D) = "white"{}

		//normal
		_NormalMap("Normal Map", 2D) = "bump"{}
		_Bumpiness("Bumpiness", Float) = 1.0

	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

	 Pass
	 {
		Tags {"LightMode" = "ForwardBase"}

		CGPROGRAM
		#pragma vertex vert 
		#pragma fragment frag 

		// make fog work
		#pragma multi_compile_fog
		#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

		#include "AutoLight.cginc" // shadow helper functions and macros
		#include "UnityLightingCommon.cginc" // for _LightColor0
		#include "UnityCG.cginc" 

		struct appdata
		{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
			float4 uv : TEXCOORD0;
			float4 tangent : TANGENT;

		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			float4 posWorld : TEXCOORD1;
			float3 normalDir : TEXCOORD9;
			float3 tangent : TEXCOORD10;
			float3 Binormal : TEXCOORD11;
			float3 lightDir : TEXCOORD2;
			float3 viewDir : TEXCOORD3;
			float4 TtoW0 : TEXCOORD4;
			float4 TtoW1 : TEXCOORD7;
			float4 TtoW2 : TEXCOORD8;
			SHADOW_COORDS(5) // for shadow 
			UNITY_FOG_COORDS(6)
		};

		//SSS≤ƒ÷ ¥¶¿Ì                                                                                                                                                                   
		inline float SubsurfaceScattering(float3 viewDir, float3 lightDir, float3 normalDir, float frontSubsurfaceDistortion, float backSubsurfaceDistortion, float frontSssIntensity)
		{
			float3 frontLitDir = normalDir * frontSubsurfaceDistortion - lightDir;
			float3 backLitDir = normalDir * backSubsurfaceDistortion + lightDir;

			float frontSSS = saturate(dot(viewDir, -frontLitDir));
			float backSSS = saturate(dot(viewDir, -backLitDir));

			float result = saturate(frontSSS * frontSssIntensity + backSSS);

			return result;
		}

		sampler2D _BasicColAndRoughnessTex;
		float4 _BasicColAndRoughnessTex_ST;
		sampler2D _InteriorColAndSpecularTex;
		sampler2D _RimLightSampler;
		sampler2D _NoiseTex;
		sampler2D _NormalMap;
		float4 _NormalMap_ST;
		float4 _Tint, _RimColor, _AmbientLight;
		float _FrontSssDistortion, _BackSssDistortion, _FrontSssIntensity, _BackSssIntensity, _InteriorColorPower;
		fixed _Bumpiness;
		float _SpecularRate, _FresnelPower, _RimIntensity, _FresnelIntensity, _UnlitRate;


		v2f vert(appdata v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.posWorld = mul(unity_ObjectToWorld, v.vertex);

			o.uv.xy = v.texcoord.xy * _BasicColAndRoughnessTex_ST.xy + _BasicColAndRoughnessTex_ST.zw;
			o.uv.zw = v.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;

			

			o.uv.xy = TRANSFORM_TEX(v.uv, _BasicColAndRoughnessTex);
			o.uv.zw = TRANSFORM_TEX(v.uv, _NormalMap);

			o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
			o.tangent = normalize(UnityObjectToWorldNormal(v.tangent.xyz));
			o.Binormal = cross(o.normalDir, o.tangent) * v.tangent.w;

			o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.posWorld.xyz);
			o.lightDir = normalize(_WorldSpaceLightPos0.xyz);

			o.TtoW0 = float4(o.tangent.x, o.Binormal.x, o.normalDir.x, o.posWorld.x);
			o.TtoW1 = float4(o.tangent.y, o.Binormal.y, o.normalDir.y, o.posWorld.y);
			o.TtoW2 = float4(o.tangent.z, o.Binormal.z, o.normalDir.z, o.posWorld.z);

			TRANSFER_SHADOW(o); // for shadow 
			UNITY_TRANSFER_FOG(o,o.pos);
			return o;

		}


		fixed4 frag(v2f i) : SV_Target
		{
			float3 worldPos = i.posWorld;

			fixed3 bump = UnpackNormal(tex2D(_NormalMap, i.uv.zw));
			bump.xy *= _Bumpiness;
			bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));

			bump = normalize(float3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));

			i.normalDir = bump;
			// common data 
			float3 NdotL = dot(i.normalDir, i.lightDir);
			float3 NdotV = dot(i.normalDir, i.viewDir);
			fixed atten = SHADOW_ATTENUATION(i); // recieve shadow 

			float lightRate = saturate((_LightColor0.x + _LightColor0.y + _LightColor0.z) * 0.3334);
			float lightingValue = saturate(NdotL.r * atten.r) * lightRate;
			float4 lightCol = lerp(_LightColor0, fixed4(1,1,1,1), 0.6);

			// sample the texture 
			fixed4 col = tex2D(_BasicColAndRoughnessTex, i.uv.xy) * _Tint;
			fixed noise = tex2D(_NoiseTex, float2(i.posWorld.x, i.posWorld.y)).r;
			fixed4 interiorSpecular = tex2D(_InteriorColAndSpecularTex, i.uv.xy);

			// Directional light SSS 
			float sssValue = SubsurfaceScattering(i.viewDir, i.lightDir, i.normalDir,  _FrontSssDistortion, _BackSssDistortion, _FrontSssIntensity);
			fixed3 sssCol = lerp(interiorSpecular, _LightColor0, saturate(pow(sssValue,_InteriorColorPower))).rgb * sssValue;
			sssCol *= _BackSssIntensity;

			// Diffuse 
			fixed4 unlitCol = col * interiorSpecular * _UnlitRate;
			fixed4 diffCol = lerp(unlitCol, col, lightingValue) * lightCol;

			// Specular
			float gloss = lerp(0.95, 0.3, interiorSpecular.a);
			float specularPow = exp2((1 - gloss) * 10.0 + 1.0);
			float3 halfVector = normalize(i.lightDir + i.viewDir);
			float3 directSpecular = pow(max(0,dot(halfVector, normalize(i.normalDir))), specularPow) * interiorSpecular.a;
			float specular = directSpecular * lerp(lightingValue, 1,0.4) * _SpecularRate;
			float noiseSpecular = lerp(specular, lerp(1 - pow(noise,specular),specular,specular), col.a);
			fixed3 specularCol = noiseSpecular * _LightColor0.rgb;

			// Side Rim 
			float falloffU = clamp(1.0 - abs(NdotV), 0.02, 0.98);
			float rimlightDot = saturate(0.5 * (dot(i.normalDir, float3(i.lightDir.z,i.lightDir.y,i.lightDir.x)) + 1.5));
			falloffU = saturate(rimlightDot * falloffU);
			falloffU = tex2D(_RimLightSampler, float2 (falloffU, 0.25f)).r;
			float3 rimCol = falloffU * _RimColor * _RimIntensity * lerp(lightingValue, 1, 0.6);

			// Fresnel
			float fresnel = 1.0 - max(0, NdotV);
			float fresnelValue = lerp(fresnel, 0, sssValue);
			float3 fresnelCol = saturate(lerp(interiorSpecular, lightCol.rgb,fresnelValue) * pow(fresnelValue, _FresnelPower) * _FresnelIntensity);

			// final color 
			fixed3 final = sssCol + diffCol.rgb + specularCol + fresnelCol + rimCol;

			// apply fog 
			UNITY_APPLY_FOG(i.fogCoord, final);

			//return fixed4(i.normalDir, 1);
			return fixed4(final, 1);

		}
			ENDCG
	 }

		// cast shadow 
	  Pass
	  {
		Tags {"LightMode" = "ShadowCaster"}
		CGPROGRAM
		#pragma vertex vert 
		#pragma fragment frag 

		#pragma multi_compile_shadowcaster 
		#include "UnityCG.cginc" 


		struct v2f
		{
			V2F_SHADOW_CASTER;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
			return o;
		}


		float4 frag(v2f i) : SV_Target
		{
			SHADOW_CASTER_FRAGMENT(i)
		}

		ENDCG
	  }
	}
}