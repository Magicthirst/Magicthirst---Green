#ifndef JITTER_FREE_PIXEL_INCLUDED
#define JITTER_FREE_PIXEL_INCLUDED

void JitterFreePixel_float(
    UnityTexture2D Tex, 
    UnitySamplerState SS, 
    float2 UV, 
    out float4 Out)
{
    // Feature: UnityTexture2D automatically exposes its bundle properties!
    // We can fetch .texelSize directly from the node's input variable.
    float4 TexelSize = Tex.texelSize; 

    // Standard Jitter-Free pixel logic
    float2 uvs = UV - (TexelSize.xy * 0.5);
    float2 uv_pixels = uvs * TexelSize.zw;
    float2 delta_pixel = frac(uv_pixels) - 0.5;

    float2 ddxy = fwidth(uv_pixels);
    float2 mip = log2(max(ddxy, 0.00001)) - 0.5;

    float2 clampedUV = uvs + (clamp(delta_pixel / max(ddxy, 0.00001), 0.0, 1.0) - delta_pixel) * TexelSize.xy;
    
    // Explicitly unwrap and pass the structural variables to the URP sampling macro
    Out = SAMPLE_TEXTURE2D_LOD(Tex.tex, SS.samplerstate, clampedUV, min(mip.x, mip.y));
}

#endif // JITTER_FREE_PIXEL_INCLUDED
