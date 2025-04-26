Shader "Unlit/VideoFullFit"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Rotation("Rotation (Degrees)", Float) = 0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                sampler2D _MainTex;
                float _Rotation;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0; // 讀取原始UV
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v)
                {
                    v2f o;

                    // 強制UV 0~1 分佈（無視原本UV）
                    float2 forcedUV = float2(v.texcoord.x, v.texcoord.y);

                    // 旋轉
                    float angle = radians(_Rotation);
                    float2 center = float2(0.5, 0.5);
                    float2 uv = forcedUV - center;
                    float2 rotatedUV = float2(
                        uv.x * cos(angle) - uv.y * sin(angle),
                        uv.x * sin(angle) + uv.y * cos(angle)
                    );
                    o.uv = rotatedUV + center;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return tex2D(_MainTex, i.uv);
                }
                ENDCG
            }
        }
}