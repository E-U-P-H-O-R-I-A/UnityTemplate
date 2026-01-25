Shader "Hidden/Glow Effect/Glow Replace" {
    SubShader
	{
        Tags { "RenderType" = "Glow" }
        Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			uniform sampler2D _GlowTex;
			uniform float4 _GlowColorMult;

			half4 frag(v2f_img i) : COLOR
			{
				return tex2D(_GlowTex,i.uv) * _GlowColorMult;
			}
			ENDCG
        } 
    } 
    
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "Transparent" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader
	{
        Tags { "RenderType" = "GlowTransparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
				
			uniform sampler2D _GlowTex;
			uniform sampler2D _MainTex;
			uniform float4 _GlowColorMult;
				
			half4 frag(v2f_img i) : COLOR
			{
				return half4((tex2D(_GlowTex,i.uv).rgb * _GlowColorMult) + .0001, tex2D(_MainTex,i.uv).a);
			}
				
			ENDCG
        } 
    }
    
    SubShader {
        Tags { "RenderType" = "TransparentCutout" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "Background" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "Overlay" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "TreeOpaque" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "TreeTransparentCutout" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "TreeBillboard" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "Grass" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }
    
    SubShader {
        Tags { "RenderType" = "GrassBillboard" }
        Pass {    
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            half4 frag(v2f_img i) : COLOR
            {
                return half4(0,0,0,0);
            }
            ENDCG
        }
    }	
	Fallback Off
}
