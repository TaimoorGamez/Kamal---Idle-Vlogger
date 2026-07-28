Shader "UI/WhiteReveal"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Reveal ("Reveal", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Reveal;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 spriteColor = tex2D(_MainTex, i.uv);

                // Alpha preserved
                float alpha = spriteColor.a;

                // Lerp from white to sprite color
                fixed3 color = lerp(fixed3(1,1,1), spriteColor.rgb, _Reveal);

                return fixed4(color, alpha);
            }
            ENDCG
        }
    }
}