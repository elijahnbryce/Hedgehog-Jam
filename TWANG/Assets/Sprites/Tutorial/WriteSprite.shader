Shader "Unlit/WriteSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MaskStep ("Mask Step", Float) = 0.1
        _T ("T", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "SpriteSortPoint"="Center" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MainTex_ST; // For scaling and tiling
            float _T;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture (letters) and mask (gradient)
                fixed4 mainColor = tex2D(_MainTex, i.uv);
                float maskValue = tex2D(_MaskTex, i.uv).r; // Use red channel of the mask

                // Determine visibility based on mask and typing progress
                float visibility = step(maskValue, _T);

                // Final alpha matches the main texture's alpha when visible
                mainColor.a *= visibility;

                return mainColor * i.color; // Modulate by vertex color for tinting
           }
            ENDCG
        }
    }
}
