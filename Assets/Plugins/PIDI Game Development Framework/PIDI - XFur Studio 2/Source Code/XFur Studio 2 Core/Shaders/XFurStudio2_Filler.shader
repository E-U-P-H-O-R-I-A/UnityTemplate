Shader "Hidden/XFur Studio 2/Designer/Filler"
{
    Properties
    {
        _MainTex ("Input Image", 2D) = "white" {}
        _UVLayout ("_UVLayout", 2D) = "white" {}
    }
   SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _UVLayout;
			half4 _MainTex_TexelSize;

			half4 frag (v2f i) : SV_Target
			{
				half alpha = tex2D(_UVLayout, i.uv).r;
				half edgeThick = 1.0;

				float4 ts = _MainTex_TexelSize;

				half a00 = tex2D(_UVLayout, i.uv + edgeThick * float2(-ts.x,-ts.y)).r;
				half a01 = tex2D(_UVLayout, i.uv + edgeThick * float2(-ts.x, ts.y)).r;
				half a10 = tex2D(_UVLayout, i.uv + edgeThick * float2(ts.x,-ts.y)).r;
				half a11 = tex2D(_UVLayout, i.uv + edgeThick * float2(ts.x, ts.y)).r;
				half2 dir = half2(a10 + a11 - a00 - a01, a01 + a11 - a00 - a10);

				half4 col = tex2D(_MainTex, i.uv);
				half4 col1 = tex2D(_MainTex, i.uv + normalize(dir) * ts.xy * edgeThick);

				if (alpha < 1)
					col = col1;
				
				return col;
			}
			ENDCG
		}
	}
}
