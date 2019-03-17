Shader "lhlv/Decal" {
  Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("Color", Color) = (1,1,1,1)
	_NormalOffset("Normal offset", Range(0, 0.01)) = 0
	
  }
  SubShader {
    Tags { "RenderType"="Opaque" "Queue"="Geometry+1" "ForceNoShadowCasting"="True" }
    LOD 200
	Cull Off
    Offset -1, -1
    
    CGPROGRAM
    #pragma surface surf Lambert decal:blend vertex:vert
    
    sampler2D _MainTex;
	fixed4 _Color;
	float _NormalOffset;
    
	
    struct Input
	{
      float2 uv_MainTex;
    };
	
	void vert(inout appdata_full v)
	{
		v.vertex.xyz += v.normal * _NormalOffset;
	}
    
    void surf (Input IN, inout SurfaceOutput o) 
	{
        half4 c = tex2D (_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb * _Color;
        o.Alpha = c.a;
      }
    ENDCG
    }
}