Shader "waterShader" {
    Properties{
        _Color("Color", Color) = (0, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
        _Amount("Extrusion Amount", float) = 1
    }
    SubShader {
        Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct v2f {
                nointerpolation float4 pos : SV_POSITION;
                nointerpolation float3 normal : NORMAL;
                nointerpolation float2 uv_MainTex : TEXCOORD0;
                nointerpolation float3 worldPos : TEXCOORD2;
            };

            uniform sampler2D _MainTex;
            uniform half4 _Color;
            uniform float _Amount;
            

            v2f vert(appdata v) {
                v2f o;
                float y = sin(_Time.y + v.vertex.x) * _Amount;
                float3 pos = v.vertex;
                o.worldPos = mul(_Object2World, v.vertex);
                v.vertex.y += sin(_Time.z + o.worldPos.x*10 + o.worldPos.z*10) * _Amount;
                o.worldPos = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.normal = v.normal;
                o.uv_MainTex = v.uv_MainTex;
                return o;
            }

            half3 computeLight(v2f i) {
                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float diffuse = 2.75 * max(.025, abs(dot(normal, lightDir)));

                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                half gloss = 2;
                float specular = 2 * pow(max(.05, abs(dot(viewDir, lightDir))), gloss);

                return diffuse*half3(.5,.5,1) + specular*half3(1,1,1);
            }

            half4 frag (v2f IN) : SV_TARGET {
                half3 light = computeLight(IN);
                half3 base = tex2D(_MainTex, IN.uv_MainTex).rgb;
                return half4(base.rgb * light, 1) * _Color;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}