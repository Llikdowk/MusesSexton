Shader "waterShader" {
    Properties{
        _albedo("Albedo", Float) = 0

        _ke("Ke", Float) = 0
        _emissiveColor("Emissive Color", Color) = (1, 1, 1, 1)

        _kd("Kd", Float) = 0
        _diffuseColor("Diffuse Color", Color) = (1, 1, 1, 1)

        _ks("Ks", Float) = 0
        _gloss("Gloss", Float) = 0
        _specularColor("Specular Color", Color) = (1, 1, 1, 1)

        _fresnelIntensity("Fresnel Intensity", Float) = 0
        _fresnelConvexity("Fresnel Convexity", Range(0, 1)) = 0
        _fresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags{ "LightMode" = "ForwardBase" "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
        Pass {
            CGPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct v2f {
                /*nointerpolation*/ float4 pos : POSITION;
                /*nointerpolation*/ float3 normal: NORMAL;
                UNITY_FOG_COORDS(1)
                /*nointerpolation*/ float4 worldPos : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            uniform float _albedo;
            uniform float _gloss;
            uniform float _ke;
            uniform float3 _emissiveColor;
            uniform float _kd;
            uniform float3 _diffuseColor;
            uniform float _ks;
            uniform float3 _specularColor;
            uniform float _fresnelIntensity;
            uniform float _fresnelConvexity;
            uniform float3 _fresnelColor;
            

            v2f vert(appdata v) {
                v2f o;
                o.worldPos = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.normal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float computeDiffuse(float3 normal, float3 lightDir) {
                float diffuse = _kd * max(_albedo, dot(normal, lightDir));
                return diffuse;
            }

            float computeSpecular(float3 position, float3 normal, float3 lightDir, float3 viewDir) {
                float3 reflectedLight = reflect(-lightDir, normal);
                float specular = _ks * pow(max(_albedo, dot(viewDir, reflectedLight)), _gloss);
                return specular;
            }

            float computeFresnel(float3 position, float3 normal, float3 viewDir) {
                float f1 = 1 - max(dot(normal, viewDir), 0);
                float f2 = 1 - f1;
                float fresnel = lerp(f1, f2, _fresnelConvexity);
                return fresnel;
            }


            float4 frag(v2f IN) : SV_TARGET
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.worldPos.xyz);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 normal = normalize(IN.normal);

                float diffuse = computeDiffuse(normal, lightDir);
                float specular = computeSpecular(IN.worldPos.xyz, normal, lightDir, viewDir);
                float fresnel = computeFresnel(IN.worldPos.xyz, normal, viewDir);

                float attenuation = LIGHT_ATTENUATION(IN);
                attenuation = ((dot(normal, lightDir)) * 0.5 + 0.5) * pow(attenuation,25)*dot(normal, lightDir);
                float3 c = _ke * _emissiveColor + attenuation*diffuse * _diffuseColor + attenuation*specular* _specularColor + fresnel * _fresnelIntensity * _fresnelColor;
                float4 col = float4(c, 1);
                UNITY_APPLY_FOG(IN.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "VertexLit"
}