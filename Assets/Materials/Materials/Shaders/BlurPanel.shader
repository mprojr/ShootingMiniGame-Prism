Shader "UI/BlurPanel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0,10)) = 3
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        GrabPass { "_BackgroundTexture" }
        
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
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            sampler2D _BackgroundTexture;
            float4 _Color;
            float _BlurSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color * _Color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float blur = _BlurSize / 100.0;
                
                fixed4 col = 0;
                float weight_total = 0;
                
                // Simple 9-tap Gaussian blur
                for(float x = -blur; x <= blur; x += blur)
                {
                    for(float y = -blur; y <= blur; y += blur)
                    {
                        float weight = 1.0;
                        weight_total += weight;
                        col += tex2Dproj(_BackgroundTexture, i.grabPos + float4(x, y, 0, 0)) * weight;
                    }
                }
                
                col /= weight_total;
                col *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
