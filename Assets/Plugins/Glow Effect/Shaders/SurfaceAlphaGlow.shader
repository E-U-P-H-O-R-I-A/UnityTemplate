Shader "Custom/SurfaceAlphaGlow" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_GlowMask ("Glow Mask Texture", 2D) = "clear" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _GlowMask;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 g = tex2D (_GlowMask, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = g.a;
		}

		ENDCG
	} 
	
	FallBack "Diffuse"
}
