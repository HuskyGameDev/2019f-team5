Shader "Custom/Debug"
{
	SubShader
	{
		Pass
		{
			Lighting Off
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			struct VertexIn
			{
				float4 vertex : POSITION;
				float4 col : COLOR;
			};

			struct VertexOut
			{
				float4 pos : POSITION;
				float4 col : COLOR;
			};

			uniform sampler2D _MainTex;

			VertexOut vert(VertexIn v)
			{
				VertexOut o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.col = v.col;

				return o;
			}

			float4 frag(VertexOut i) : COLOR
			{
				return i.col;
			}

			ENDCG
		}
	}
}