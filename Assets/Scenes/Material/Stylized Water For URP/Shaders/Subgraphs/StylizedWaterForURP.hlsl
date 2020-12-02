// Designed & Developed by Alexander Ameye
// https://alexander-ameye.gitbook.io/stylized-water/
// Version 1.0.3

#ifndef STYLIZED_WATER_FOR_URP_INCLUDED
    #define STYLIZED_WATER_FOR_URP_INCLUDED

    #define pi 3.14

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ▛ ▘▘▘▘▘▘▘▘▘▘▜ 																													    
    //   UTILITY FUNCTIONS																														    
    // ▙ ▖▖▖▖▖▖▖▖▖▖▟	 				
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    float2 Panner(float2 uv, float direction, float speed, float2 offset, float tiling)
    {
        direction = direction * 2 - 1;
        float2 dir = normalize(float2(cos(pi * direction), sin(pi * direction)));
        return  (dir * _Time.y * speed) + offset + (uv * tiling);
    }

    float2 Distort(float2 UV, float Amount)
    {
        float time = _Time.y;
        
        UV.y += Amount * 0.01 * (sin(UV.x * 3.5 + time * 0.35) + sin(UV.x * 4.8 + time * 1.05) + sin(UV.x * 7.3 + time * 0.45)) / 3.0;
        UV.x += Amount * 0.12 * (sin(UV.y * 4.0 + time * 0.5) + sin(UV.y * 6.8 + time * 0.75) + sin(UV.y * 11.3 + time * 0.2)) / 3.0;
        UV.y += Amount * 0.12 * (sin(UV.x * 4.2 + time * 0.64) + sin(UV.x * 6.3 + time * 1.65) + sin(UV.x * 8.2 + time * 0.45)) / 3.0;

        return UV;
    }

    float2 GradientNoiseDir(float2 p)
    {   
        p = p % 289;
        float x = (34 * p.x + 1) * p.x % 289 + p.y;
        x = (34 * x + 1) * x % 289;
        x = frac(x / 41) * 2 - 1;
        return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
    }

    float GradientNoise(float2 p)
    {
        float2 ip = floor(p);
        float2 fp = frac(p);
        float d00 = dot(GradientNoiseDir(ip), fp);
        float d01 = dot(GradientNoiseDir(ip + float2(0, 1)), fp - float2(0, 1));
        float d10 = dot(GradientNoiseDir(ip + float2(1, 0)), fp - float2(1, 0));
        float d11 = dot(GradientNoiseDir(ip + float2(1, 1)), fp - float2(1, 1));
        fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
        return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
    }

    float4 Remap(float4 In, float2 InMinMax, float2 OutMinMax)
    {
        return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
    }

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ▛ ▘▘▘▘▘▜ 																													    
    //   LIGHTING																														    
    // ▙ ▖▖▖▖▟	 				
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    void Specular_float(float3 positionWS, float roughness, float3 normalDir, float3 viewDir, out float3 Specular)
    {
        #if SHADERGRAPH_PREVIEW
            Specular = 0;
        #else
            Light mainLight = GetMainLight();
            float3 lightDir = GetMainLight().direction;
            float3 L = normalize(lightDir);
            float3 N = normalize(normalDir);
            float3 V = normalize(viewDir);

            roughness = roughness / 250;

            float Roughness2 = roughness * roughness;
            float3 H = SafeNormalize(L + V);
            float NdotH = saturate(dot(N, H)); 
            float LdotH = saturate(dot(L, H));
            float d = NdotH * NdotH * (Roughness2 - 1.h) + 1.0001h;
            float LdotH2 = LdotH * LdotH;
            float specularTerm = Roughness2 / ((d * d) * max(0.1h, LdotH2) * (roughness + 0.5h) * 4);
            #if defined (SHADER_API_MOBILE)
                specularTerm = specularTerm - HALF_MIN;
                specularTerm = clamp(specularTerm, 0.0, 5.0); 
            #endif
            Specular = specularTerm * mainLight.color * mainLight.distanceAttenuation;
        #endif
    }

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ▛ ▘▘▘▘▘▜ 																													    
    //   CAUSTICS																														    
    // ▙ ▖▖▖▖▟	 				
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    float3 RGBSplit(float Split, Texture2D Texture, SamplerState Sampler, float2 UV)
    {
        float2 UVR = UV + float2(Split, Split);
        float2 UVG = UV + float2(Split, -Split);
        float2 UVB = UV + float2(-Split, -Split);

        float r = SAMPLE_TEXTURE2D(Texture, Sampler, UVR).r;
        float g = SAMPLE_TEXTURE2D(Texture, Sampler, UVG).g;
        float b = SAMPLE_TEXTURE2D(Texture, Sampler, UVB).b;

        return float3(r,g,b);
    }

    void CausticsTexture_float(float3 sceneColor, float2 uv, SamplerState Sampler, float4 visuals, float strength, Texture2D Texture, out float3 Out)
    {
        float luminance = visuals.x;
        float split = visuals.y * 0.01;
        float speed = visuals.z;
        float scale = visuals.w;

        float3 texture_1 = RGBSplit(split, Texture, Sampler, Panner(uv, 1, speed, float2(0,0), 1/scale));
        float3 texture_2 = RGBSplit(split, Texture, Sampler, Panner(uv, 1, speed, float2(0,0), -1/scale));
        float3 texture_combined = min(texture_1, texture_2);
        float luminance_mask = lerp(1, Luminance(sceneColor), luminance);

        Out = luminance_mask * strength * 10 * texture_combined;
    }

    void CausticsTextureMobile_float(float2 uv, SamplerState Sampler, float4 visuals, float strength, Texture2D Texture, out float3 Out)
    {
        float split = visuals.y * 0.01;
        float speed = visuals.z;
        float scale = visuals.w;

        float3 texture_1 = RGBSplit(split, Texture, Sampler, Panner(uv, 1, speed, float2(0,0), 1/scale));
        float3 texture_2 = RGBSplit(split, Texture, Sampler, Panner(uv, 1, speed, float2(0,0), -1/scale));
        float3 texture_combined = min(texture_1, texture_2);

        Out = strength * 10 * texture_combined;
    }

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ▛ ▘▘▘▜ 																													    
    //   FOAM																														    
    // ▙ ▖▖▖▟	 				
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    float4 SurfaceFoam(float2 uv, float2 movement, float2 offset, float scale, float3 sampling, SamplerState Sampler, Texture2D Texture)
    {
        float direction = movement.x;
        float speed = movement.y;
        float cutoff = sampling.x;
        float softness = sampling.y;
        float distortion = sampling.z;

        float2 DistortedUV = Distort(Panner(uv, direction, speed, offset, 1/scale), distortion);

        float edge1 = cutoff - softness;
        float edge2 = cutoff + softness;

        return smoothstep(edge1, edge2, SAMPLE_TEXTURE2D(Texture, Sampler, DistortedUV));
    }

    void SurfaceFoam_float(float2 uv, float4 movement, float2 scale, float2 offset, float3 sampling, SamplerState Sampler, Texture2D Texture, out float Primary, out float Secondary)
    {
        Primary = SurfaceFoam(uv, movement.xy, 0, scale.x, sampling, Sampler, Texture).r;
        Secondary = SurfaceFoam(uv, movement.zw, offset, scale.y, sampling, Sampler, Texture).g;
    }

    void FoamUVs_float(float2 uv, float4 movement, float2 offset, float2 scale, float depth, out float4 Shoreline, out float4 Custom)
    {
        float direction1 = movement.x;
        float direction2 = movement.z;
        float speed1 = movement.y;
        float speed2 = movement.w;
        float scale1 = scale.x;
        float scale2 = scale.y;

        float2 shoreline_uv = float2(uv.x * 0.2, depth);
        Shoreline.xy = Panner(shoreline_uv, 0.5, speed1, float2(0,0), 1/scale1);
        Shoreline.zw = Panner(shoreline_uv, 0.5, speed2, float2(0,0), 1/scale2);

        Custom.xy = Panner(uv, direction1, speed1, float2(0,0), 1/scale1);
        Custom.zw = Panner(uv, direction2, speed2, offset, 1/scale2);
    }

    void FoamSampling_float(float4 uvs, float cutoff, float distortion, SamplerState Sampler, Texture2D Texture, out float Primary, out float Secondary)
    {
        Primary   = saturate(step(cutoff, SAMPLE_TEXTURE2D(Texture, Sampler,  Distort(uvs.xy, distortion)).r));
        Secondary = saturate(step(cutoff, SAMPLE_TEXTURE2D(Texture, Sampler, Distort(uvs.zw, distortion)).g));
    }

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ▛ ▘▘▘▘▘▘▜ 																													    
    //   REFRACTION																														    
    // ▙ ▖▖▖▖▖▖▟	 				
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    void RefractUV_float(float2 uv, float3 parameters, out float2 Out)
    {
        float speed = parameters.x;
        float scale = parameters.y;
        float strength = parameters.z;

        float2 PanningUV = Panner(uv, 1, speed, 0, 1/scale);
        float4 Noise = GradientNoise(PanningUV);
        float2 RemappedNoise = Remap(Noise, float2(0,1), float2(-1,1));
        float2 CameraSize = float2(unity_OrthoParams.x, unity_OrthoParams.y);

        Out = (RemappedNoise * strength) / CameraSize;
    }

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ▛ ▘▘▘▘▜ 																													    
    //   NORMALS																														    
    // ▙ ▖▖▖▖▟	 				
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    float3 NormalStrength(float3 In, float Strength)
    {
        return float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
    }

    float3 UnpackNormals(float4 packedNormal, float scale = 1.0)
    {
        packedNormal.a *= packedNormal.r;
        float3 normal;
        normal.xy = packedNormal.ag * 2.0 - 1.0;
        normal.xy *= scale;
        normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
        return normal;
    }

    void Normals_float(float2 uv, Texture2D Texture, SamplerState Sampler, float scale, float strength, float speed, out float3 Out)
    {
        float2 UV1 = Panner(uv, 1, speed, float2(0,0), 1/scale);
        float2 UV2 = Panner(uv, 1, -speed, float2(0,0), 1/scale * 0.5);
        float4 Texture1 = SAMPLE_TEXTURE2D(Texture, Sampler, UV1);
        float4 Texture2 = SAMPLE_TEXTURE2D(Texture, Sampler, UV2);
        Texture1.rgb = UnpackNormals(Texture1);
        Texture2.rgb = UnpackNormals(Texture2);
        float3 Strength1 = NormalStrength(Texture1.rgb, strength);
        float3 Strength2 = NormalStrength(Texture2.rgb, strength);

        Out = normalize(float3(Strength1.rg + Strength2.rg, Strength1.b * Strength2.b));
    }

    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
    // ▛ ▘▘▘▜ 																													    
    //   WAVES																														    
    // ▙ ▖▖▖▟	 				
    //━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

    float3 Gerstner(float3 position, float steepness, float wavelength, float speed, float direction, inout float3 tangent, inout float3 binormal)
    {
        direction = direction * 2 - 1;
        float2 d = normalize(float2(cos(pi * direction), sin(pi * direction)));
        float s = steepness;
        float k = 2 * pi / wavelength;                                                      
        float f = k * (dot(d, position.xz) - speed * _Time.y);
        float a = s / k;

        tangent += float3(
        -d.x * d.x * s * sin(f),
        d.x * s * cos(f), 
        -d.x * d.y * s * sin(f)
        );

        binormal += float3(
        -d.x * d.y * s * sin(f),
        d.y * s * cos(f),
        -d.y * d.y * s * sin(f)
        );

        return float3(
        d.x * a * cos(f),
        a * sin(f),
        d.y * a * cos(f)
        );
    }

    void GerstnerWaves_float(float3 p, float3 visuals, float4 directions, out float3 Offset, out float3 normal, out float3x3 TBN)
    {
        float steepness = visuals.x ;
        float wavelength = visuals.y;
        float speed = visuals.z;

        Offset = float3(0,0,0);
        float3 tangent = float3(1, 0, 0);
        float3 binormal = float3(0, 0, 1);

        Offset += Gerstner(p, steepness, wavelength, speed, directions.x, tangent, binormal);
        Offset +=  Gerstner(p, steepness, wavelength, speed, directions.y, tangent, binormal);
        Offset +=  Gerstner(p, steepness, wavelength, speed, directions.z, tangent, binormal);
        Offset +=  Gerstner(p, steepness, wavelength, speed, directions.w, tangent, binormal);

        normal = normalize(cross(binormal, tangent));
        TBN = transpose(float3x3(tangent, binormal, normal));
    }
#endif