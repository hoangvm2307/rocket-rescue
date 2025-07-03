// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/FloatingObject"
{
    Properties
    {
    	_Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Gradient ("Gradient (RGB)", 2D) = "white" {}
		_Depth ("Depth", float) = 10
    }
    SubShader
    {
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"

            uniform sampler2D _MainTex;  
			uniform sampler2D _Gradient;
			uniform float _Depth;
			fixed4 _Color;

			struct appdata 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				half2 texcoord : TEXCOORD;
			};

            struct v2f
            {
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(5) // put shadows data into TEXCOORD1
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
                float4 pos : SV_POSITION;
                float2 uv2 : TEXCOORD3;
            };
            v2f vert (appdata_base v)
            {
                v2f o;
                float4 wPos = mul(unity_ObjectToWorld, v.vertex);
//                o.pos = UnityObjectToClipPos(v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, wPos);;


                o.uv = v.texcoord;
                o.uv2 = half2(((wPos.y + _Depth) / _Depth), 0);
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                // compute shadows data
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	fixed3 ramp = tex2D(_Gradient, i.uv2);
                fixed4 col = tex2D(_MainTex, i.uv);
                // compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                fixed shadow = SHADOW_ATTENUATION(i);
                // darken light's illumination with shadow, keep ambient intact
                fixed3 lighting = ((i.diff * shadow) + i.ambient) * (ramp) * _Color;
//                fixed3 lighting = (i.diff + shadow) * ramp;

                col.rgb *= lighting;
                return col;
            }
            ENDCG
        }
        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}