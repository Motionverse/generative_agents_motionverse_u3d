// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Suntail Surface"
{
	Properties
	{
		[SingleLineTexture][Header(Maps)][Space(10)][MainTexture]_Albedo("Albedo", 2D) = "white" {}
		[Normal][SingleLineTexture]_Normal("Normal", 2D) = "bump" {}
		[SingleLineTexture]_MetallicSmoothness("Metallic/Smoothness", 2D) = "white" {}
		[HDR][SingleLineTexture]_Emission("Emission", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1
		[Header(Settings)][Space(5)]_Color("Color", Color) = (1,1,1,0)
		[HDR]_EmissionColor("Emission", Color) = (0,0,0,1)
		_NormalScale("Normal", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_SurfaceSmoothness("Smoothness", Range( 0 , 1)) = 0
		[KeywordEnum(Metallic_Alpha,Albedo_Alpha)] _SmoothnessSource("Smoothness Source", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_local _SMOOTHNESSSOURCE_METALLIC_ALPHA _SMOOTHNESSSOURCE_ALBEDO_ALPHA
		#pragma multi_compile __ LOD_FADE_CROSSFADE
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float _Tiling;
		uniform float _NormalScale;
		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform sampler2D _Emission;
		uniform float4 _EmissionColor;
		uniform float _Metallic;
		uniform sampler2D _MetallicSmoothness;
		uniform float _SurfaceSmoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_Tiling).xx;
			float2 uv_TexCoord250 = i.uv_texcoord * temp_cast_0;
			float2 Tiling252 = uv_TexCoord250;
			float3 Normal75 = UnpackScaleNormal( tex2D( _Normal, Tiling252 ), _NormalScale );
			o.Normal = Normal75;
			float4 tex2DNode2 = tex2D( _Albedo, Tiling252 );
			float4 Albedo19 = ( _Color * tex2DNode2 );
			o.Albedo = Albedo19.rgb;
			float4 Emission259 = ( tex2D( _Emission, Tiling252 ) * _EmissionColor );
			o.Emission = Emission259.rgb;
			float4 tex2DNode239 = tex2D( _MetallicSmoothness, Tiling252 );
			float Metallic262 = ( _Metallic * tex2DNode239.r );
			o.Metallic = Metallic262;
			float AlbedoSmoothness267 = tex2DNode2.a;
			#if defined(_SMOOTHNESSSOURCE_METALLIC_ALPHA)
				float staticSwitch266 = tex2DNode239.a;
			#elif defined(_SMOOTHNESSSOURCE_ALBEDO_ALPHA)
				float staticSwitch266 = AlbedoSmoothness267;
			#else
				float staticSwitch266 = tex2DNode239.a;
			#endif
			float Smoothness263 = ( staticSwitch266 * _SurfaceSmoothness );
			o.Smoothness = Smoothness263;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18909
251;73;1885;924;4200.064;826.8273;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;254;-1612.259,-526.3455;Inherit;False;708.7898;344.8789;;3;252;250;248;Tiling;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;248;-1554.065,-371.4539;Inherit;False;Property;_Tiling;Tiling;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;250;-1381.065,-388.4539;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;41;-3766.895,-517.7137;Inherit;False;1066.593;525.2227;;6;19;3;2;1;255;267;Albedo;0.5180138,0.6980392,0.4901961,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;252;-1120.067,-393.4539;Inherit;False;Tiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;255;-3707.348,-224.2908;Inherit;False;252;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-3505.932,-249.1109;Inherit;True;Property;_Albedo;Albedo;0;1;[SingleLineTexture];Create;True;0;0;0;False;3;Header(Maps);Space(10);MainTexture;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;79;-4077.257,83.87437;Inherit;False;1376.693;541.9077;;10;242;262;241;54;268;266;239;257;263;240;Metallic/Smoothness;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;267;-3179.47,-153.7058;Inherit;False;AlbedoSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;260;-2623.188,-91.06023;Inherit;False;1060.835;556.2388;;5;259;258;244;245;243;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;257;-4042.854,296.9248;Inherit;False;252;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;80;-2633.932,-516.5142;Inherit;False;947.0427;336.418;;4;75;175;256;6;Normal;0.6251274,0.49,0.7,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;268;-3783.368,479.2715;Inherit;False;267;AlbedoSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;239;-3855.599,272.9095;Inherit;True;Property;_MetallicSmoothness;Metallic/Smoothness;2;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;258;-2562.067,39.39619;Inherit;False;252;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;256;-2534.97,-431.8682;Inherit;False;252;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-3408.912,498.6462;Inherit;False;Property;_SurfaceSmoothness;Smoothness;9;0;Create;False;0;0;0;False;0;False;0;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-3410.059,222.8964;Inherit;False;Property;_Metallic;Metallic;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-2503.55,-327.3289;Inherit;False;Property;_NormalScale;Normal;7;0;Create;False;0;1;Option1;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;266;-3424.959,369.4602;Inherit;False;Property;_SmoothnessSource;Smoothness Source;10;0;Create;True;0;0;0;True;0;False;1;0;0;True;;KeywordEnum;2;Metallic_Alpha;Albedo_Alpha;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-3422.544,-433.361;Inherit;False;Property;_Color;Color;5;0;Create;False;0;0;0;False;2;Header(Settings);Space(5);False;1,1,1,0;0.65,0.5223636,0.4549999,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;245;-2268.63,240.7933;Inherit;False;Property;_EmissionColor;Emission;6;1;[HDR];Create;False;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;243;-2360.212,15.76204;Inherit;True;Property;_Emission;Emission;3;2;[HDR];[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-2287.294,-420.1191;Inherit;True;Property;_Normal;Normal;1;2;[Normal];[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-1987.73,134.0931;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-3075.658,277.2964;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-3132.314,-351.2796;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-3075.657,413.2965;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;262;-2907.48,272.5011;Inherit;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-2945.363,-355.6335;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-2907.48,408.5011;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-1952.751,-420.3694;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-1807.939,130.5096;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-1314.853,27.22835;Inherit;False;19;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-1330.625,414.3497;Inherit;False;263;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-1313.315,127.1985;Inherit;False;75;Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;261;-1313.632,233.1606;Inherit;False;259;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;264;-1316.625,323.3496;Inherit;False;262;Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1073.929,157.8826;Float;False;True;-1;2;;0;0;Standard;Raygeas/Suntail Surface;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;198;-1;0;True;191;1;Pragma;multi_compile __ LOD_FADE_CROSSFADE;False;;Custom;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;250;0;248;0
WireConnection;252;0;250;0
WireConnection;2;1;255;0
WireConnection;267;0;2;4
WireConnection;239;1;257;0
WireConnection;266;1;239;4
WireConnection;266;0;268;0
WireConnection;243;1;258;0
WireConnection;6;1;256;0
WireConnection;6;5;175;0
WireConnection;244;0;243;0
WireConnection;244;1;245;0
WireConnection;241;0;242;0
WireConnection;241;1;239;1
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;240;0;266;0
WireConnection;240;1;54;0
WireConnection;262;0;241;0
WireConnection;19;0;3;0
WireConnection;263;0;240;0
WireConnection;75;0;6;0
WireConnection;259;0;244;0
WireConnection;0;0;20;0
WireConnection;0;1;77;0
WireConnection;0;2;261;0
WireConnection;0;3;264;0
WireConnection;0;4;265;0
ASEEND*/
//CHKSM=8517FF88576200EC4F3A85C2AD689F34353AD81B