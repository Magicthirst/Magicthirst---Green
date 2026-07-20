Shader "Sprites/JitterFreeUnlit"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                
                // Native Unity Sprites use Premultiplied Alpha
                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            float4 texturePointSmooth(sampler2D tex, float2 uvs)
            {
                // Shift UVs to coordinate pixel centers safely
                uvs -= _MainTex_TexelSize.xy * 0.5;
                float2 uv_pixels = uvs * _MainTex_TexelSize.zw;
                float2 delta_pixel = frac(uv_pixels) - 0.5;

                // Screen derivative calculation
                float2 ddxy = fwidth(uv_pixels);
                
                // Safe Guard 1: Prevent log2(0) crashes when calculating the mipmap levels
                float2 mip = log2(max(ddxy, 0.00001)) - 0.5;

                // Safe Guard 2: Prevent Division-By-Zero NaN artifacts if ddxy equals zero
                float2 interpolationFactor = clamp(delta_pixel / max(ddxy, 0.00001), 0.0, 1.0);
                
                float2 clampedUV = uvs + (interpolationFactor - delta_pixel) * _MainTex_TexelSize.xy;
                
                return tex2Dlod(tex, float4(clampedUV, 0, min(mip.x, mip.y)));
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = texturePointSmooth(_MainTex, IN.texcoord) * IN.color;
                
                // Apply Sprite tint color multiplication logic
                c.rgb *= c.a; 
                return c;
            }
            ENDCG
        }
    }
}
