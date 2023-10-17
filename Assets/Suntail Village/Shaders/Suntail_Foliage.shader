// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Suntail Foliage"
{
	Properties
	{
		[SingleLineTexture][Header(Maps)][Space(10)][MainTexture]_Albedo("Albedo", 2D) = "white" {}
		[SingleLineTexture]_SmoothnessTexture("Smoothness", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1
		[Header(Settings)][Space(5)]_MainColor("Main Color", Color) = (1,1,1,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.35
		[Header(Second Color Settings)][Space(5)][Toggle(_COLOR2ENABLE_ON)] _Color2Enable("Enable", Float) = 0
		_SecondColor("Second Color", Color) = (0,0,0,0)
		[KeywordEnum(World_Position,UV_Based)] _SecondColorOverlayType("Overlay Type", Float) = 0
		_SecondColorOffset("Offset", Float) = 0
		_SecondColorFade("Fade", Range( -1 , 1)) = 0.5
		_WorldScale("World Scale", Float) = 1
		[Header(Wind Settings)][Space(5)][Toggle(_ENABLEWIND_ON)] _EnableWind("Enable", Float) = 1
		_WindForce("Force", Range( 0 , 1)) = 0.3
		_WindWavesScale("Waves Scale", Range( 0 , 1)) = 0.25
		_WindSpeed("Speed", Range( 0 , 1)) = 0.5
		[Toggle(_ANCHORTHEFOLIAGEBASE_ON)] _Anchorthefoliagebase("Anchor the foliage base", Float) = 0
		[Header(Lighting Settings)][Space(5)]_DirectLightOffset("Direct Light Offset", Range( 0 , 1)) = 0
		_DirectLightInt("Direct Light Int", Range( 1 , 10)) = 1
		_IndirectLightInt("Indirect Light Int", Range( 1 , 10)) = 1
		_TranslucencyInt("Translucency Int", Range( 0 , 100)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ENABLEWIND_ON
		#pragma shader_feature_local _ANCHORTHEFOLIAGEBASE_ON
		#pragma shader_feature_local _COLOR2ENABLE_ON
		#pragma shader_feature_local _SECONDCOLOROVERLAYTYPE_WORLD_POSITION _SECONDCOLOROVERLAYTYPE_UV_BASED
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float4 screenPosition;
			float3 worldNormal;
			INTERNAL_DATA
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _WindSpeed;
		uniform float _WindWavesScale;
		uniform float _WindForce;
		uniform sampler2D _Albedo;
		uniform float _Tiling;
		uniform float4 _MainColor;
		uniform float4 _SecondColor;
		uniform float _WorldScale;
		uniform float _SecondColorOffset;
		uniform float _SecondColorFade;
		uniform float _IndirectLightInt;
		uniform float _DirectLightOffset;
		uniform float _DirectLightInt;
		uniform float _TranslucencyInt;
		uniform sampler2D _SmoothnessTexture;
		uniform float _Smoothness;
		uniform float _AlphaCutoff;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime34 = _Time.y * ( _WindSpeed * 5 );
			float simplePerlin3D35 = snoise( ( ase_worldPos + mulTime34 )*_WindWavesScale );
			float temp_output_231_0 = ( simplePerlin3D35 * 0.01 );
			#ifdef _ANCHORTHEFOLIAGEBASE_ON
				float staticSwitch376 = ( temp_output_231_0 * pow( v.texcoord.xy.y , 2.0 ) );
			#else
				float staticSwitch376 = temp_output_231_0;
			#endif
			#ifdef _ENABLEWIND_ON
				float staticSwitch341 = ( staticSwitch376 * ( _WindForce * 30 ) );
			#else
				float staticSwitch341 = 0.0;
			#endif
			float Wind191 = staticSwitch341;
			float3 temp_cast_0 = (Wind191).xxx;
			v.vertex.xyz += temp_cast_0;
			v.vertex.w = 1;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 temp_cast_0 = (_Tiling).xx;
			float2 uv_TexCoord747 = i.uv_texcoord * temp_cast_0;
			float2 Tiling748 = uv_TexCoord747;
			float4 tex2DNode1 = tex2D( _Albedo, Tiling748 );
			float OpacityMask263 = tex2DNode1.a;
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen760 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither760 = Dither4x4Bayer( fmod(clipScreen760.x, 4), fmod(clipScreen760.y, 4) );
			dither760 = step( dither760, ( unity_LODFade.x > 0.0 ? unity_LODFade.x : 1.0 ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			UnityGI gi436 = gi;
			float3 diffNorm436 = ase_worldNormal;
			gi436 = UnityGI_Base( data, 1, diffNorm436 );
			float3 indirectDiffuse436 = gi436.indirect.diffuse + diffNorm436 * 0.0001;
			float4 temp_output_10_0 = ( _MainColor * tex2DNode1 );
			float3 ase_worldPos = i.worldPos;
			float simplePerlin3D742 = snoise( ase_worldPos*_WorldScale );
			simplePerlin3D742 = simplePerlin3D742*0.5 + 0.5;
			#if defined(_SECONDCOLOROVERLAYTYPE_WORLD_POSITION)
				float staticSwitch360 = simplePerlin3D742;
			#elif defined(_SECONDCOLOROVERLAYTYPE_UV_BASED)
				float staticSwitch360 = i.uv_texcoord.y;
			#else
				float staticSwitch360 = simplePerlin3D742;
			#endif
			float SecondColorMask335 = saturate( ( ( staticSwitch360 + _SecondColorOffset ) * ( _SecondColorFade * 2 ) ) );
			float4 lerpResult332 = lerp( temp_output_10_0 , ( _SecondColor * tex2D( _Albedo, Tiling748 ) ) , SecondColorMask335);
			#ifdef _COLOR2ENABLE_ON
				float4 staticSwitch340 = lerpResult332;
			#else
				float4 staticSwitch340 = temp_output_10_0;
			#endif
			float4 Albedo259 = staticSwitch340;
			float4 IndirectLight612 = ( float4( indirectDiffuse436 , 0.0 ) * Albedo259 * _IndirectLightInt );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float dotResult430 = dot( ase_worldlightDir , ase_normWorldNormal );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 DirectLight614 = ( ( saturate( (dotResult430*1.0 + _DirectLightOffset) ) * ase_lightAtten ) * ase_lightColor * Albedo259 * _DirectLightInt );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult461 = dot( ase_worldlightDir , ase_worldViewDir );
			float TranslucencyMask660 = (-dotResult461*1.0 + -0.2);
			float dotResult672 = dot( ase_worldlightDir , ase_normWorldNormal );
			float4 Translucency631 = saturate( ( ( TranslucencyMask660 * ( ( ( (dotResult672*1.0 + 1.0) * ase_lightAtten ) * ase_lightColor * Albedo259 ) * 0.25 ) ) * _TranslucencyInt ) );
			SurfaceOutputStandard s677 = (SurfaceOutputStandard ) 0;
			s677.Albedo = float3( 0,0,0 );
			s677.Normal = ase_worldNormal;
			s677.Emission = float3( 0,0,0 );
			s677.Metallic = 0.0;
			s677.Smoothness = ( tex2D( _SmoothnessTexture, Tiling748 ) * _Smoothness ).r;
			s677.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi677 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g677 = UnityGlossyEnvironmentSetup( s677.Smoothness, data.worldViewDir, s677.Normal, float3(0,0,0));
			gi677 = UnityGlobalIllumination( data, s677.Occlusion, s677.Normal, g677 );
			#endif

			float3 surfResult677 = LightingStandard ( s677, viewDir, gi677 ).rgb;
			surfResult677 += s677.Emission;

			#ifdef UNITY_PASS_FORWARDADD//677
			surfResult677 -= s677.Emission;
			#endif//677
			float3 Smoothness734 = saturate( surfResult677 );
			c.rgb = ( IndirectLight612 + DirectLight614 + Translucency631 + float4( Smoothness734 , 0.0 ) ).rgb;
			c.a = 1;
			clip( ( OpacityMask263 * dither760 ) - _AlphaCutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.screenPosition;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.screenPosition = IN.customPack2.xyzw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18909
255;73;1881;924;7376.818;2481.502;3.328203;True;False
Node;AmplifyShaderEditor.CommentaryNode;749;-2493.029,351.6331;Inherit;False;688.9;314.4999;;3;748;747;746;Tiling;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;336;-3272.233,-1478.755;Inherit;False;1921.852;761.7965;;12;335;334;382;391;377;310;360;248;742;361;743;745;Second Color Mask;0.7,0.7,0.7,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;745;-3176.628,-1390.033;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;743;-3160.938,-1211.943;Inherit;False;Property;_WorldScale;World Scale;11;0;Create;True;0;0;0;False;0;False;1;3.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;746;-2468.205,491.303;Inherit;False;Property;_Tiling;Tiling;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;742;-2931.296,-1312.366;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;361;-2902.202,-1060.226;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;747;-2272.552,472.4474;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;360;-2615.841,-1170.937;Inherit;False;Property;_SecondColorOverlayType;Overlay Type;8;0;Create;False;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;World_Position;UV_Based;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;748;-2002.553,467.4474;Inherit;False;Tiling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;248;-2435.594,-863.7374;Inherit;False;Property;_SecondColorFade;Fade;10;0;Create;False;0;0;0;False;0;False;0.5;0.82;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;262;-5464.229,-1485;Inherit;False;2097.364;882.0443;;18;156;263;259;340;332;367;10;337;1;3;366;247;754;368;753;751;752;750;Albedo;0.5177868,0.7,0.49,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-2470.071,-1007.13;Inherit;False;Property;_SecondColorOffset;Offset;9;0;Create;False;0;0;0;False;0;False;0;-0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;377;-2200.386,-1087.577;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;750;-5422.28,-965.8592;Inherit;False;748;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleNode;391;-2134.751,-859.2667;Inherit;False;2;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;751;-5208.28,-1024.859;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;752;-5202.28,-791.8592;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;382;-1936.126,-978.8624;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;753;-4978.459,-1048.044;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;368;-5210.191,-1022.857;Inherit;True;Property;_Albedo;Albedo;0;1;[SingleLineTexture];Create;True;0;0;0;False;3;Header(Maps);Space(10);MainTexture;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.WireNode;754;-4994.459,-784.0435;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;334;-1771.428,-978.316;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;335;-1594.897,-982.6814;Inherit;False;SecondColorMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;247;-4815.405,-1025.55;Inherit;False;Property;_SecondColor;Second Color;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.02352945,0.2,0.02000001,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-4899.858,-1229.814;Inherit;True;Property;_LeavesTexture;Leaves Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;456;-5464.92,344.5705;Inherit;False;2898.686;1981.707;;5;668;627;733;626;630;Lighting;0.7,0.686289,0.49,1;0;0
Node;AmplifyShaderEditor.SamplerNode;366;-4899.025,-839.039;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-4818.124,-1419.369;Inherit;False;Property;_MainColor;Main Color;3;0;Create;True;0;0;0;False;2;Header(Settings);Space(5);False;1,1,1,0;0.1992481,0.5,0.4172932,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;668;-5367.252,466.0584;Inherit;False;1214.467;434.7671;;7;660;464;465;463;461;460;459;Translucency Mask;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;630;-5364.178,1747.631;Inherit;False;2228.953;513.9716;;17;631;655;666;470;662;667;661;469;648;676;468;619;674;672;673;675;671;Translucency;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;66;-5464.272,-503.3702;Inherit;False;2680.3;740.17;;18;191;341;188;376;345;56;359;356;231;35;358;357;190;182;228;34;344;36;Wind;0.49,0.6290355,0.7,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;367;-4498.595,-936.7111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;337;-4516.115,-1106.451;Inherit;False;335;SecondColorMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-4495.411,-1330.155;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;460;-5254.958,679.077;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;459;-5317.251,516.0585;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;671;-5289.08,1961.358;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;36;-5429.632,-246.7836;Inherit;False;Property;_WindSpeed;Speed;15;0;Create;False;0;0;0;False;0;False;0.5;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;332;-4228.949,-1149.796;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;675;-5318.858,1813.359;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;340;-3897.969,-1336.19;Inherit;False;Property;_Color2Enable;Enable;6;0;Create;False;0;0;0;False;2;Header(Second Color Settings);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;461;-5003.466,596.9058;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;672;-5016.081,1881.358;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;673;-5064.961,2020.758;Inherit;False;Constant;_DirectLightOffset2;Direct Light Offset 2;18;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;344;-5132.634,-241.392;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-3610.187,-1336.708;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;674;-4767.812,1882.491;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;465;-4888.985,820.882;Inherit;False;Constant;_TranslucencyOffset;Translucency Offset;19;0;Create;True;0;0;0;False;0;False;-0.2;-0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;627;-5368.805,969.2724;Inherit;False;1679.855;716.6;;13;614;649;638;434;433;457;431;435;432;430;429;428;427;Direct Light;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.LightAttenuation;619;-4771.391,2029.227;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;463;-4789.863,597.5899;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-4962.952,-241.6264;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;228;-4962.759,-409.6157;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;676;-4492.942,1934.697;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-4866.769,-126.1545;Inherit;False;Property;_WindWavesScale;Waves Scale;14;0;Create;False;0;0;0;False;0;False;0.25;0.411;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;182;-4716.685,-329.3925;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;468;-4512.123,2044.98;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;648;-4529.854,2169.948;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;733;-4106.14,468.7854;Inherit;False;1479.169;401.6033;;7;734;706;677;709;681;708;755;Smoothness;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;428;-5331.526,1035.002;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;427;-5283.747,1199;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScaleAndOffsetNode;464;-4627.222,686.1621;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;35;-4503.841,-238.515;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;430;-5028.748,1113;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;755;-4066.504,554.5702;Inherit;False;748;Tiling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;469;-4287.479,2021.668;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;429;-5097.826,1262.879;Inherit;False;Property;_DirectLightOffset;Direct Light Offset;17;0;Create;True;0;0;0;False;2;Header(Lighting Settings);Space(5);False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;660;-4399.301,680.7759;Inherit;False;TranslucencyMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;358;-4402.588,132.5774;Inherit;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-4481.184,2.437496;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;681;-3844.91,758.1143;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;661;-4214.327,1909.024;Inherit;False;660;TranslucencyMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;432;-4780.479,1114.133;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;708;-3865.462,529.6811;Inherit;True;Property;_SmoothnessTexture;Smoothness;1;1;[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;667;-4116.114,2021.295;Inherit;False;0.25;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;356;-4195.656,22.40697;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;231;-4185.764,-234.2584;Inherit;False;0.01;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3949.659,38.50756;Inherit;False;Property;_WindForce;Force;13;0;Create;False;0;0;0;False;0;False;0.3;0.48;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;359;-3986.644,-116.5151;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;435;-4546.492,1114.031;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;662;-3919.313,1951.472;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;626;-3632.499,972.9844;Inherit;False;933.2305;378.5339;;5;612;441;436;438;458;Indirect Light;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;709;-3494.461,655.6807;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;470;-4049.085,2114.592;Inherit;False;Property;_TranslucencyInt;Translucency Int;20;0;Create;True;0;0;0;False;0;False;1;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;431;-4589.965,1312.194;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;666;-3721.006,2017.475;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;436;-3548.723,1022.985;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;638;-4328.551,1235.873;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;433;-4341.605,1386.068;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CustomStandardSurface;677;-3314.014,562.9437;Inherit;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;457;-4453.205,1585.23;Inherit;False;Property;_DirectLightInt;Direct Light Int;18;0;Create;True;0;0;0;False;0;False;1;0;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;458;-3582.498,1206.08;Inherit;False;Property;_IndirectLightInt;Indirect Light Int;19;0;Create;True;0;0;0;False;0;False;1;0;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;434;-4359.882,1505.049;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;345;-3662.742,44.30282;Inherit;False;30;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;376;-3793.085,-237.1686;Inherit;False;Property;_Anchorthefoliagebase;Anchor the foliage base;16;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;438;-3491.725,1116.984;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-3461.298,-114.811;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;655;-3547.248,2017.634;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LODFadeNode;756;-2526.774,-357.0073;Inherit;False;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;706;-3029.917,563.4495;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;649;-4098.947,1406.893;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;441;-3223.978,1098.518;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;631;-3368.36,2012.636;Inherit;False;Translucency;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;763;-2304.901,-384.2354;Inherit;False;2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;734;-2833.088,559.2493;Inherit;False;Smoothness;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-4526.999,-1188.709;Inherit;False;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;614;-3916.375,1401.929;Inherit;False;DirectLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;612;-2942.268,1093.473;Inherit;False;IndirectLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;341;-3278.123,-144.9097;Inherit;False;Property;_EnableWind;Enable;12;0;Create;False;0;0;0;False;2;Header(Wind Settings);Space(5);False;0;1;1;True;;Toggle;2;;;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-3012.979,-143.4771;Inherit;False;Wind;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;634;-2071.992,-61.00904;Inherit;False;631;Translucency;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;735;-2067.137,21.28462;Inherit;False;734;Smoothness;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;615;-2059.907,-146.4264;Inherit;False;614;DirectLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;267;-2067.885,-443.6967;Inherit;False;263;OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;613;-2068,-234.3004;Inherit;False;612;IndirectLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DitheringNode;760;-2065.975,-327.4391;Inherit;False;0;False;4;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;3;SAMPLERSTATE;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-4591.908,-725.2592;Inherit;False;Property;_AlphaCutoff;Alpha Cutoff;5;0;Create;False;0;0;0;False;0;False;0.35;0.45;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;236;-1821.479,57.95247;Inherit;False;191;Wind;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;759;-1811.975,-390.4391;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;452;-1803.187,-134.8504;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;151;-1593.555,-363.9565;Float;False;True;-1;2;;0;0;CustomLighting;Raygeas/Suntail Foliage;False;False;False;False;False;False;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;3;False;558;1;False;559;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;16;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;4;1;False;-1;1;False;-1;1;False;-1;1;False;-1;0;False;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;156;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;742;0;745;0
WireConnection;742;1;743;0
WireConnection;747;0;746;0
WireConnection;360;1;742;0
WireConnection;360;0;361;2
WireConnection;748;0;747;0
WireConnection;377;0;360;0
WireConnection;377;1;310;0
WireConnection;391;0;248;0
WireConnection;751;0;750;0
WireConnection;752;0;750;0
WireConnection;382;0;377;0
WireConnection;382;1;391;0
WireConnection;753;0;751;0
WireConnection;754;0;752;0
WireConnection;334;0;382;0
WireConnection;335;0;334;0
WireConnection;1;0;368;0
WireConnection;1;1;753;0
WireConnection;366;0;368;0
WireConnection;366;1;754;0
WireConnection;367;0;247;0
WireConnection;367;1;366;0
WireConnection;10;0;3;0
WireConnection;10;1;1;0
WireConnection;332;0;10;0
WireConnection;332;1;367;0
WireConnection;332;2;337;0
WireConnection;340;1;10;0
WireConnection;340;0;332;0
WireConnection;461;0;459;0
WireConnection;461;1;460;0
WireConnection;672;0;675;0
WireConnection;672;1;671;0
WireConnection;344;0;36;0
WireConnection;259;0;340;0
WireConnection;674;0;672;0
WireConnection;674;2;673;0
WireConnection;463;0;461;0
WireConnection;34;0;344;0
WireConnection;676;0;674;0
WireConnection;676;1;619;0
WireConnection;182;0;228;0
WireConnection;182;1;34;0
WireConnection;464;0;463;0
WireConnection;464;2;465;0
WireConnection;35;0;182;0
WireConnection;35;1;190;0
WireConnection;430;0;428;0
WireConnection;430;1;427;0
WireConnection;469;0;676;0
WireConnection;469;1;468;0
WireConnection;469;2;648;0
WireConnection;660;0;464;0
WireConnection;432;0;430;0
WireConnection;432;2;429;0
WireConnection;708;1;755;0
WireConnection;667;0;469;0
WireConnection;356;0;357;2
WireConnection;356;1;358;0
WireConnection;231;0;35;0
WireConnection;359;0;231;0
WireConnection;359;1;356;0
WireConnection;435;0;432;0
WireConnection;662;0;661;0
WireConnection;662;1;667;0
WireConnection;709;0;708;0
WireConnection;709;1;681;0
WireConnection;666;0;662;0
WireConnection;666;1;470;0
WireConnection;638;0;435;0
WireConnection;638;1;431;0
WireConnection;677;4;709;0
WireConnection;345;0;56;0
WireConnection;376;1;231;0
WireConnection;376;0;359;0
WireConnection;188;0;376;0
WireConnection;188;1;345;0
WireConnection;655;0;666;0
WireConnection;706;0;677;0
WireConnection;649;0;638;0
WireConnection;649;1;433;0
WireConnection;649;2;434;0
WireConnection;649;3;457;0
WireConnection;441;0;436;0
WireConnection;441;1;438;0
WireConnection;441;2;458;0
WireConnection;631;0;655;0
WireConnection;763;0;756;1
WireConnection;763;2;756;1
WireConnection;734;0;706;0
WireConnection;263;0;1;4
WireConnection;614;0;649;0
WireConnection;612;0;441;0
WireConnection;341;0;188;0
WireConnection;191;0;341;0
WireConnection;760;0;763;0
WireConnection;759;0;267;0
WireConnection;759;1;760;0
WireConnection;452;0;613;0
WireConnection;452;1;615;0
WireConnection;452;2;634;0
WireConnection;452;3;735;0
WireConnection;151;10;759;0
WireConnection;151;13;452;0
WireConnection;151;11;236;0
ASEEND*/
//CHKSM=B17816C1880189654141FA57DE9F709086324B84