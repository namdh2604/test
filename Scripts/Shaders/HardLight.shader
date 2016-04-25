Shader "Custom/Blending/Hard Light" 
{
	//https://bitbucket.org/maho125/hardlight/src/afbbcbdf2e3823ef1b4e0e300a252d5c25923eca/Assets/Shaders/Hard%20Light.shader?at=master
	//http://answers.unity3d.com/questions/1017666/how-to-write-shader-for-hard-light-blend-mode-in-u.html
	//This shader was pulled from a UnityAnswers Post the reference links are above
	
	Properties 
	{
		_Color ("Tint", Color) = (1,1,1,1)
		_Opacity ("Opacity", Range(0,1)) = 1.0
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags
		{
			"Queue" = "Transparent"  
			"RenderType" = "Transparent"
		}
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		Cull Off
		ZWrite Off
		Lighting Off
		Fog { Mode Off }
		
		GrabPass 
		{
			"_sharedGrabTexture"
		}
		
		Pass 
		{

	        CGPROGRAM
		    
			#include "UnityCG.cginc"
		    
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			sampler2D _sharedGrabTexture;
			float _Opacity;
			
			fixed4 _Color;
			
			struct vertInput
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 uv : TEXCOORD0;
			};
			
			struct vertOutput
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 grab_uv: TEXCOORD1;
				float2 uv : TEXCOORD0;  
			};
			
			vertOutput vert(vertInput input)
			{
			
				vertOutput output;
			    
				output.vertex = mul(UNITY_MATRIX_MVP,input.vertex);
				output.grab_uv = ComputeGrabScreenPos(output.vertex);
				output.uv = input.uv.xy;
				output.color = input.color;
			    
				return output;
			}
			
			float4 HardLight (float4 a, float4 b)
			{
				float4 o = b >= .5 ? 1.0 - 2 * (1.0 - b) * (1.0 - a) : 2.0 * a * b;
				o.a = b.a;
				return o;
			}
			
			float4 frag(vertOutput input) : SV_Target 
			{
			
				float4 texture_color = tex2D(_MainTex, input.uv) * _Color;
			  
				#if UNITY_UV_STARTS_AT_TOP
					input.grab_uv.y = 1.0 - input.grab_uv.y;
				#endif

				float4 scene_color = tex2D(_sharedGrabTexture, input.grab_uv.xy) * _Color;
	
				float4 output = HardLight(scene_color, texture_color);
			   
				output = lerp(texture_color,output,_Opacity);
			   
				return output;
			}
			
			ENDCG 
 
		} 

	}
	
	Fallback Off
}

