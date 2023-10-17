Shader "Liquid/SimpleLiquidShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_SurfaceLevel("Liquid Surface Level", Vector) = (0,1,0,0)
		_GravityDirection("Gravity direction",Vector) = (0,-1,0,0)
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Zwrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"

			struct appdata 
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 worldPos : POSITION1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _GravityDirection;
			float4 _SurfaceLevel;
			fixed4 _Color;

			v2f vert(appdata v)
			{
	
				v2f o;
	//
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	//
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
			{
				float dotProd1 = dot(_SurfaceLevel - i.worldPos, _GravityDirection);
				if (dotProd1 > 0) discard;

				return _Color;
			}
		ENDCG
		}
	}
}