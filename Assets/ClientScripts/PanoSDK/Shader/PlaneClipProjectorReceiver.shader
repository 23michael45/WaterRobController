// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/PlaneClipProjectorReceiver" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	
	_planePos	("Clipping Plane Position",	Vector)	= ( 0, 0, 0, 1 )
	_planeNorm	("Clipping Plane Normal",	Vector)	= ( 0, 1, 0, 1 )
}

SubShader {
	Tags {"Queue"="Opaque"}
	LOD 100
	
	Cull Off
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float4 wpos : TEXCOORD01;
				float4 opos : TEXCOORD02;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float4 _planePos;
			float4 _planeNorm;
			
			
			float distanceToPlane(float3 planePosition, float3 planeNormal, float3 pointInWorld)
			{
			  //w = vector from plane to point
			  float3 w = - ( planePosition - pointInWorld );
			  return ( 
				planeNormal.x * w.x + 
				planeNormal.y * w.y + 
				planeNormal.z * w.z 
			  ) / sqrt ( 
				planeNormal.x * planeNormal.x +
				planeNormal.y * planeNormal.y +
				planeNormal.z * planeNormal.z 
			  );
			}
			void PlaneClip(float3 posWorld)
			{
				clip(distanceToPlane(_planePos.xyz, _planeNorm.xyz, posWorld));
			}
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.opos = v.vertex;
				o.wpos = mul(unity_ObjectToWorld, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				PlaneClip(i.wpos);
				
				
				return col;
			}
		ENDCG
	}
}

}
