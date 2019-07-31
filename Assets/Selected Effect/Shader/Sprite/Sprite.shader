// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Selected Effect --- Outline/Sprite" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_Color ("Tint", Color) = (1, 1, 1, 1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[Header(Outline)]
		_OutlineColor         ("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineThickness     ("Outline Thickness", Float) = 1
		_OutlineDashSpeed     ("Outline Dash Speed", Float) = 30
		_OutlineDashGap       ("Outline Dash Gap", Float) = 1
		_OutlineDashThickness ("Outline Dash Thickness", Float) = 0.5
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Sprite"
			"CanUseSpriteAtlas" = "True"
		}
		Pass
		{
			Cull Off Lighting Off ZWrite Off
			Blend One OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ OUTLINE_ONLY
			#pragma multi_compile _ OUTLINE_DASH
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color, _OutlineColor, _MainTex_TexelSize;
			fixed _OutlineThickness, _OutlineDashSpeed, _OutlineDashGap, _OutlineDashThickness;
			
			struct v2f
			{
				float4 pos   : SV_POSITION;
				fixed4 color : COLOR;
				float2 uv    : TEXCOORD0;
			};
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.color = v.color * _Color;
#ifdef PIXELSNAP_ON
				o.pos = UnityPixelSnap(o.pos);
#endif
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb *= c.a;
#if OUTLINE_DASH
				float2 v = i.pos.xy + _Time * _OutlineDashSpeed;
				float skip = sin(_OutlineDashGap * abs(distance(0.5, v))) + _OutlineDashThickness;
				fixed4 oc = _OutlineColor;
				oc.a = lerp(0.0, 1.0, saturate(skip));
#else
				fixed4 oc = _OutlineColor;
#endif
				oc.a *= ceil(c.a);
				oc.rgb *= oc.a;
 
				fixed up = tex2D(_MainTex,    i.uv + fixed2(0, _MainTex_TexelSize.y * _OutlineThickness)).a;
				fixed down = tex2D(_MainTex,  i.uv - fixed2(0, _MainTex_TexelSize.y * _OutlineThickness)).a;
				fixed right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x * _OutlineThickness, 0)).a;
				fixed left = tex2D(_MainTex,  i.uv - fixed2(_MainTex_TexelSize.x * _OutlineThickness, 0)).a;
#if OUTLINE_ONLY
				c = fixed4(0, 0, 0, 0);
#endif
				return lerp(oc, c, ceil(up * down * right * left));
			}
			ENDCG
		}
	}
	Fallback Off
}