Shader "Hidden/XFur Studio 2/Basic Shells 4"
{
	Properties{
		[PerRendererData] _XFurSelfStrandsPattern("Strands Pattern", 2D) = "white"{}
		[PerRendererData] _XFurParamData("XFur Params Data", 2D) = "white"{}
		[PerRendererData] _XFurSelfColorVariation("XFur Color Variation Map", 2D) = "black"{}
		[PerRendererData] _XFurSelfColor("XFur Main Tint", Color) = (1,1,1,1)
		[PerRendererData] _XFurSelfColorA("XFur Color Variation A", Color) = (1,1,1,1)
		[PerRendererData] _XFurSelfColorB("XFur Color Variation B", Color) = (1,1,1,1)
		[PerRendererData] _XFurSelfColorC("XFur Color Variation C", Color) = (1,1,1,1)
		[PerRendererData] _XFurSelfColorD("XFur Color Variation D", Color) = (1,1,1,1)
		[PerRendererData] _XFurUVTiling("XFur Strands Tiling", Float) = 2
		[PerRendererData] _XFurProfilesSplat("Profiles Splat Map", 2D) = "black"{}
		[PerRendererData] _XFurCullFur("Cull Fur", Float) = 2
		[PerRendererData] _XFurHasGroomData("Has Groom Data", Float) = 0
		[PerRendererData] _XFurVFXMask("VFX Mask", 2D) = "black"{}
		[PerRendererData] _XFurPhysics("XFur Physics", 2D) = "black"{}
		[PerRendererData] _XFurSkinMap("XFur Skin Color Map", 2D) = "black"{}
		[PerRendererData] _XFurSkinNormal("XFur Skin Normal Map", 2D) = "bump"{}
		[PerRendererData] _XFurSkinColor("XFur Skin Color", Color) = (1,1,1,1)
		[PerRendererData] _XFurSkinSmoothness("XFur Skin Smoothness", Range(0,1)) = 0.3
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Cull[_XFurCullFur]


		CGPROGRAM

		#pragma target 3.0
		#pragma surface surf Standard fullforwardshadows addshadow vertex:SimpleVert

		#define FUR_PASS -1
		#include "CGIncludes/XFurStudio2_StandardPasses.cginc"

		half _XFurHasNormal;

		void surf(Input IN, inout SurfaceOutputStandard o) {
		  o.Albedo = tex2D(_XFurSkinMap, IN.xfurUVs.xy).rgb * _XFurSkinColor;
		  o.Normal = UnpackNormal(tex2D(_XFurSkinNormal, IN.xfurUVs.xy));
		  o.Smoothness = _XFurSkinSmoothness;
		}

		ENDCG

		CGPROGRAM

		#pragma target 3.0
		#pragma surface BasicShellSurfacePass Standard fullforwardshadows vertex:BasicShellVert addshadow
		#pragma shader_feature_local FEATURESET_MOBILE
		#pragma shader_feature_local FURPROFILES_BLENDED
		#pragma shader_feature_local FURDATA_BAKED

#define FUR_PASS 0

		#include "CGIncludes/XFurStudio2_StandardPasses.cginc"

		ENDCG

		CGPROGRAM

		#pragma target 3.0
		#pragma surface BasicShellSurfacePass Standard fullforwardshadows vertex:BasicShellVert addshadow
		#pragma shader_feature_local FEATURESET_MOBILE
		#pragma shader_feature_local FURPROFILES_BLENDED
		#pragma shader_feature_local FURDATA_BAKED

#define FUR_PASS 1

		#include "CGIncludes/XFurStudio2_StandardPasses.cginc"

		ENDCG


		CGPROGRAM

		#pragma target 3.0
		#pragma surface BasicShellSurfacePass Standard fullforwardshadows vertex:BasicShellVert addshadow
		#pragma shader_feature_local FEATURESET_MOBILE
		#pragma shader_feature_local FURPROFILES_BLENDED
		#pragma shader_feature_local FURDATA_BAKED

#define FUR_PASS 2

		#include "CGIncludes/XFurStudio2_StandardPasses.cginc"

		ENDCG

		CGPROGRAM

		#pragma target 3.0
		#pragma surface BasicShellSurfacePass Standard fullforwardshadows vertex:BasicShellVert addshadow
		#pragma shader_feature_local FEATURESET_MOBILE
		#pragma shader_feature_local FURPROFILES_BLENDED
		#pragma shader_feature_local FURDATA_BAKED

#define FUR_PASS 3

		#include "CGIncludes/XFurStudio2_StandardPasses.cginc"

		ENDCG


	}


}
