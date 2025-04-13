// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Shader "Hidden/Fronkon Games/Artistic/Sharpen URP"
{
  Properties
  {
    _MainTex("Main Texture", 2D) = "white" {}
  }

  SubShader
  {
    Tags
    {
      "RenderType" = "Opaque"
      "RenderPipeline" = "UniversalPipeline"
    }
    LOD 100
    ZTest Always ZWrite Off Cull Off

    Pass
    {
      Name "Fronkon Games Artistic Color Sharpen Luma"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ LUMA_FAST LUMA_NORMAL LUMA_WIDER LUMA_PYRAMID
      #pragma multi_compile ___ DEBUG_VIEW
      //#pragma multi_compile ___ ARTISTIC_DEMO

      #include "Artistic.hlsl"
      #include "Vibrance.hlsl"

      float _Sharpness;
      float _SharpClamp;
      float _OffsetBias;

      half4 ArtisticFrag(const VertexOutput input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.uv).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        float3 luma = float3(0.2126, 0.7152, 0.0722) * _Sharpness;
#if LUMA_FAST
        pixel.rgb = SAMPLE_MAIN(uv + (_MainTex_TexelSize.xy / 3.0) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + (-_MainTex_TexelSize.xy / 3.0) * _OffsetBias).rgb;

        pixel.rgb *= 0.5;
        luma *= 1.5;
#elif LUMA_NORMAL
        pixel.rgb  = SAMPLE_MAIN(uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * 0.5 * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - _MainTex_TexelSize.xy * 0.5 * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + _MainTex_TexelSize.xy * 0.5 * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * 0.5 * _OffsetBias).rgb;

        pixel.rgb *= 0.25;
#elif LUMA_WIDER
        pixel.rgb  = SAMPLE_MAIN(uv + _MainTex_TexelSize.xy * float2(0.4, -1.2) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - _MainTex_TexelSize.xy * float2(1.2, 0.4) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + _MainTex_TexelSize.xy * float2(1.2, 0.4) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - _MainTex_TexelSize.xy * float2(0.4, -1.2) * _OffsetBias).rgb;

        pixel.rgb *= 0.25;
        luma *= 0.51;
#else
        pixel.rgb  = SAMPLE_MAIN(uv + float2(0.5 * _MainTex_TexelSize.x, -_MainTex_TexelSize.y * _OffsetBias)).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + float2(_OffsetBias * -_MainTex_TexelSize.x, 0.5 * -_MainTex_TexelSize.y)).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + float2(_OffsetBias * _MainTex_TexelSize.x, 0.5 * _MainTex_TexelSize.y)).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + float2(0.5 * -_MainTex_TexelSize.x, _MainTex_TexelSize.y * _OffsetBias)).rgb;
        //pixel.rgb += (2.0 * pixel.rgb);

        pixel.rgb *= 0.25;
        luma *= 0.666;
#endif
        half3 sharp = color.rgb - pixel.rgb;

        float4 sharpClamp = float4(luma * (0.5 / _SharpClamp), 0.5);

        float sharpLuma = saturate(dot(float4(sharp, 1.0), sharpClamp));
        sharpLuma = (_SharpClamp * 2.0) * sharpLuma - _SharpClamp;

        pixel.rgb += sharpLuma;
        pixel.rgb = saturate(pixel.rgb);

#if DEBUG_VIEW
        return color * saturate(sharpLuma * 5.0);
#endif
        pixel.rgb = Vibrance(pixel.rgb);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

#if ARTISTIC_DEMO
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif

        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Color Sharpen Contrast Adaptive"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ DEBUG_VIEW
      //#pragma multi_compile ___ ARTISTIC_DEMO

      #include "Artistic.hlsl"
      #include "Vibrance.hlsl"

      float _Sharpness;
      float _OffsetBias;

      half4 ArtisticFrag(const VertexOutput input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.uv).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        float2 off = _MainTex_TexelSize.xy * _OffsetBias;
        float3 a = SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb;
        float3 b = SAMPLE_MAIN(uv + float2(0.0,    -off.y)).rgb;
        float3 c = SAMPLE_MAIN(uv + float2(off.x,  -off.y)).rgb;
        float3 d = SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb;
        float3 f = SAMPLE_MAIN(uv + float2(off.x,  0.0)).rgb;
        float3 g = SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb;
        float3 h = SAMPLE_MAIN(uv + float2(0.0,    off.y)).rgb;
        float3 i = SAMPLE_MAIN(uv + float2(off.x,  off.y)).rgb;

        float3 mnRGB = min(min(min(d, color.rgb), min(f, b)), h);
        float3 mnRGB2 = min(mnRGB, min(min(a, c), min(g, i)));
        mnRGB += mnRGB2;

        float3 mxRGB = max(max(max(d, color.rgb), max(f, b)), h);
        float3 mxRGB2 = max(mxRGB, max(max(a, c), max(g, i)));
        mxRGB += mxRGB2;

        float3 rcpMRGB = rcp(mxRGB);
        float3 ampRGB = saturate(min(mnRGB, 2.0 - mxRGB) * rcpMRGB);    

        ampRGB = rsqrt(ampRGB);

        float peak = -3.0 * _Sharpness + 8.0;
        float3 wRGB = -rcp(ampRGB * peak);

        float3 rcpWeightRGB = rcp(4.0 * wRGB + 1.0);

        float3 window = (b + d) + (f + h);
        pixel.rgb = saturate((window * wRGB + color.rgb) * rcpWeightRGB);

#if DEBUG_VIEW
        return color * sqrt((color.r - pixel.r) + (color.g - pixel.g) + (color.b - pixel.b));
#endif
        pixel.rgb = Vibrance(pixel.rgb);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

#if ARTISTIC_DEMO
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif

        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }   
  }
  
  FallBack "Diffuse"
}
