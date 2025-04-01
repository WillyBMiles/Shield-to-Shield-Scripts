 Shader "Transparent/Diffuse ZWrite" {
 Properties {
     _Color ("Main Colour", Color) = (1,1,1,1)
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 }
 SubShader {
     Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     LOD 200
 
     // Initial pass renders to depth buffer only
     Pass {
         ZWrite On
         ColorMask 0
     }
 
     // Now use regular forward rendering passes from Transparent/Diffuse
     UsePass "Transparent/Diffuse/FORWARD"
 }
 Fallback "Transparent/VertexLit"
 }
 