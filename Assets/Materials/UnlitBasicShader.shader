// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/UnlitBasicShader"
{
    Properties
    {

        _Color("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
        _ColorCenterOfMass("Color of the center of mass", Color) = (1.0, 1.0, 1.0, 1.0)
        _SizeCenterOfMass("Size of the center of mass", Float) = 1
        
    }
    SubShader
    {
        Tags { "Queue" = "Transparent"  "RenderType"="Transparent" "DisableBatching" = "True"}
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

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
            };

            struct v2f
            {
                
                
                float4 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };



            float4 _Color;
            float4 _ColorCenterOfMass;
            float _SizeCenterOfMass;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                
              
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 localSpace = i.worldPos - mul(unity_ObjectToWorld,float4(0,0,0,1)).xyz;
                float distance = sqrt(localSpace.x * localSpace.x + localSpace.y * localSpace.y);
                float circleCenterOfMass = step(distance,_SizeCenterOfMass);
                
                
                fixed4 col = lerp(_Color, _ColorCenterOfMass, circleCenterOfMass);

                
                 
                return col;
            }
            ENDCG
        }
    }
}
