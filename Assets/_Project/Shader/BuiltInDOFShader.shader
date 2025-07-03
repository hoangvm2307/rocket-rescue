// Made by Gemini - Dành cho Built-in Render Pipeline
Shader "Hidden/BuiltInDOFShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FocusDistance("Focus Distance", Range(0.1, 100)) = 10
        _FocusRange("Focus Range", Range(0, 100)) = 3
        _BlurAmount("Blur Amount", Range(0, 5)) = 1
    }
    SubShader
    {
        // Chạy shader trên một quad bao phủ toàn màn hình
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // Include file quan trọng cho Built-in

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

            // Các biến và texture cần thiết
            sampler2D _MainTex;
            sampler2D_float _CameraDepthTexture; // Dùng sampler2D_float cho depth
            float4 _MainTex_TexelSize;

            float _FocusDistance;
            float _FocusRange;
            float _BlurAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Bước 1: Lấy độ sâu
                float rawDepth = tex2D(_CameraDepthTexture, i.uv).r;
                float sceneDepth = LinearEyeDepth(rawDepth);

                // Bước 2: Tính toán độ mờ (tương tự URP)
                float coc = abs(sceneDepth - _FocusDistance);
                coc = saturate((coc - _FocusRange) / coc);
                float blur = coc * _BlurAmount;
                
                fixed4 originalColor = tex2D(_MainTex, i.uv);

                // Bước 3: Thực hiện blur nếu cần
                if (blur > 0.01)
                {
                    fixed4 blurredColor = 0;
                    // Thuật toán Box Blur đơn giản
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            float2 offset = float2(x, y) * _MainTex_TexelSize.xy * blur;
                            blurredColor += tex2D(_MainTex, i.uv + offset);
                        }
                    }
                    return blurredColor / 9.0;
                }
                
                return originalColor;
            }
            ENDCG
        }
    }
}