Shader "CrushLights" {
	Properties {
	 	_MainTex ("", 2D) = "white" {}
	 	_IsRaven ("", Float) = 0.0
	 	_DarkThreshold ("", Float) = 0.3
	 	_BrightThreshold ("", Float) = 0.7
	}
	SubShader {
	 	Pass {
	  		CGPROGRAM
	  		#pragma vertex vert_img
	  		#pragma fragment frag
	  		#include "UnityCG.cginc"

	  		uniform sampler2D _MainTex;
	  		uniform float _IsRaven;
	  		uniform float _DarkThreshold;
	  		uniform float _BrightThreshold;

			float luma (fixed3 color) {
				return dot(color, fixed3(0.8, 0.8, 0.8));
			}

	  		fixed4 frag (v2f_img i) : COLOR {
	   			fixed3 col = tex2D (_MainTex, i.uv).rgb;

	   			if (_IsRaven == 1.0) {
	   				if (luma(col) > _BrightThreshold) return fixed4(1.0, 1.0, 1.0, 1.0);
	   			} else {
	   				if (luma(col) < _DarkThreshold) return fixed4(0.0, 0.0, 0.0, 0.0);
	   			}
				
				return fixed4(col, 1.0);
	  		}
	  		ENDCG
	 	}
	}
	Fallback "Diffuse"
}