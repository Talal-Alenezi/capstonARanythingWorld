/*
 * Copyright (c) 2019 Dummiesman
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
*/

using AnythingWorld;
using Dummiesman;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class MTLLoader
{
    public List<string> SearchPaths = new List<string>() { "%FileName%_Textures", string.Empty };
    private int GetArgValueCount(string arg)
    {
        switch (arg)
        {
            case "-bm":
            case "-clamp":
            case "-blendu":
            case "-blendv":
            case "-imfchan":
            case "-texres":
                return 1;
            case "-mm":
                return 2;
            case "-o":
            case "-s":
            case "-t":
                return 3;
        }
        return -1;
    }

    private int GetTexNameIndex(string[] components)
    {
        for (var i = 1; i < components.Length; i++)
        {
            var cmpSkip = GetArgValueCount(components[i]);
            if (cmpSkip < 0)
            {
                return i;
            }
            i += cmpSkip;
        }
        return -1;
    }

    private float GetArgValue(string[] components, string arg, float fallback = 1f)
    {
        var argLower = arg.ToLower();
        for (var i = 1; i < components.Length - 1; i++)
        {
            var cmp = components[i].ToLower();
            if (argLower == cmp)
            {
                return OBJLoaderHelper.FastFloatParse(components[i + 1]);
            }
        }
        return fallback;
    }

    private string GetTexPathFromMapStatement(string processedLine, string[] splitLine)
    {
        var texNameCmpIdx = GetTexNameIndex(splitLine);
        if (texNameCmpIdx < 0)
        {
            Debug.LogError($"texNameCmpIdx < 0 on line {processedLine}. Texture not loaded.");
            return null;
        }
        var texNameIdx = processedLine.IndexOf(splitLine[texNameCmpIdx]);
        var texturePath = processedLine.Substring(texNameIdx);
        return texturePath;
    }
    private bool TryGetVectorArgFromSplitLine(string processedLine, string[] splitline, string arg, out Vector3 argVector)
    {
        argVector = Vector3.zero;
        //Debug.Log("looking for arg " + arg +" in "+processedLine);
        var argIndex = GetArgIndex(arg, splitline, processedLine);
        if (argIndex > 0)
        {

            var argSubstring = processedLine.Substring(argIndex);
            //
            var argSplit = argSubstring.Split(' ');

            Debug.Log($"Argsplit length = {argSplit.Length}");
            Debug.Log(argSubstring);
            if (argSplit.Length >= 3)
            {
                for (var i = 1; i < 4; i++)
                {
                    var result = OBJLoaderHelper.FastFloatParse(argSplit[i]);
                    if (result == float.NaN) return false;
                    Debug.Log($"Index {i} with value {argSplit[i]} being parsed to {result}");
                    argVector[i - 1] = result;
                }
                return true;
            }
        }
        return false;
    }
    private int GetArgIndex(string arg, string[] splitline, string processedLine)
    {
        var argLower = arg.ToLower();
        for (var i = 1; i < splitline.Length - 1; i++)
        {
            var cmp = splitline[i].ToLower();
            if (argLower == cmp)
            {
                var processedLineIndx = processedLine.IndexOf(cmp);
                return processedLineIndx;
            }
        }
        return -1;
    }


    public Dictionary<string, Material> Load(Stream input, Dictionary<string, Texture> textures)
    {
        var reader = new StreamReader(input);
        var mtlDict = new Dictionary<string, Material>();
        Material currentMaterial = null;
        var dValueAssigned = false;
        string line;
        var mtl_file = "MATERIAL FILE" + "\n";
        var mtl_log = "MATERIAL LOADING LOG" + "\n";
        while ((line = reader.ReadLine()) != null)
        {
            mtl_file = mtl_file + line + "\n";
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var processedLine = line.Clean();
            var splitLine = processedLine.Split(' ');

            //blank or comment
            if (splitLine.Length < 2 || processedLine[0] == '#')
                continue;


            //newmtl
            if (splitLine[0] == "newmtl")
            {
                var materialName = processedLine.Substring(7);
                var newMtl = new Material(AnythingSettings.Instance.DefaultUnstitchedShader) { name = materialName };
                mtlDict[materialName] = newMtl;
                currentMaterial = newMtl;
                dValueAssigned = false;
                continue;
            }

            //anything past here requires a material instance
            if (currentMaterial == null)
                continue;

            //diffuse color
            if (splitLine[0] == "Kd" || splitLine[0] == "kd")
            {
                var currentColor = currentMaterial.GetColor("_BaseColor");
                var kdColor = OBJLoaderHelper.ColorFromStrArray(splitLine);
                mtl_log = mtl_log + "Base color (Kd): " + $"{kdColor.r} {kdColor.g} {kdColor.b} {currentColor.a}" + "\n";

                currentMaterial.SetColor("_BaseColor", new Color(kdColor.r, kdColor.g, kdColor.b, currentColor.a));
                //Debug.Log($"{kdColor.r} {kdColor.g} {kdColor.b}");
                continue;
            }

            // diffuse map
            if (splitLine[0] == "map_Kd" || splitLine[0] == "map_kd" || splitLine[0] == "map-Kd" || splitLine[0] == "map-kd")
            {
                var texturePath = GetTexPathFromMapStatement(processedLine, splitLine);

                if(TryGetVectorArgFromSplitLine(processedLine,splitLine,"-s", out var scaleVector))
                {
                    currentMaterial.SetVector("_Tiling", scaleVector);
                }
                if(TryGetVectorArgFromSplitLine(processedLine,splitLine,"-o", out var offsetVector))
                {
                    currentMaterial.SetVector("_Offset", offsetVector);
                }

                //Debug.Log("MTLLOADER tex path:" + texturePath);
                if (texturePath == null)
                {
                    Debug.Log("map_Kd specified but no texturePath found");
                    continue;
                }
                else
                {

                }


                if (textures.ContainsKey(texturePath))
                {
                    Texture loadedTex;
                    textures.TryGetValue(texturePath, out loadedTex);


                    if (loadedTex != null)
                    {
                        var KdTexture = loadedTex as Texture2D;
                        //KdTexture.alphaIsTransparency = true;
                        if (KdTexture != null)
                        {
                            currentMaterial.SetTexture("_BaseMap", KdTexture);
                            mtl_log = mtl_log + "_BaseMap (map_Kd): " + texturePath + "\n";
                        }
                        else
                        {
                            Debug.Log("Error loading texture" + texturePath + " to texture 2D");
                            continue;
                        }

                        //WORK AROUND: set kd to white if texture present and is pitch black
                        // if things are turning white instead of black this is the culprit. apologies.


                        if (currentMaterial.GetColor("_BaseColor") == Color.black)
                        {
                            if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning("Warning: Base color is black, overriding to white.");
                            currentMaterial.SetColor("_BaseColor", Color.white);
                            //if (AnythingSettings.Instance.showDebugMessages) Debug.LogError("Setting basecolor to white!");
                            //

                            // mtl_log = mtl_log + "Switched base color from black to white" + "\n";
                        }
                    }
                    else
                    {
                        Debug.Log("Texture key \"" + texturePath + "\" found in dictionary but texture was not loaded.");
                        mtl_log = mtl_log + "Error: Base map " + texturePath + " found, error loading" + "\n";
                    }

                }
                else
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Texture \"" + texturePath + "\" not found in dictionary");

                    mtl_log = mtl_log + "Error: Base map " + texturePath + " not found, not loaded." + "\n";

                    continue;
                }

                /*
                if (Path.GetExtension(texturePath).ToLower() == ".dds")
                {
                    currentMaterial.mainTextureScale = new Vector2(1f, -1f);
                }
                */
                continue;
            }

            // bump map
            if (splitLine[0] == "map_Bump" || splitLine[0] == "map_bump")
            {
                var texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                if (texturePath == null)
                {
                    continue; //invalid args or sth
                }

                if (textures.ContainsKey(texturePath))
                {
                    textures.TryGetValue(texturePath, out var loadedTex);
                    if (loadedTex != null)
                    {
                        var bumpTex = loadedTex as Texture2D;
                        if (bumpTex != null)
                        {
                            currentMaterial.SetTexture("_BumpMap", bumpTex);

                            var bumpScale = GetArgValue(splitLine, "-bm", 1.0f);
                            currentMaterial.SetFloat("_BumpScale", bumpScale);
                            currentMaterial.EnableKeyword("_NORMALMAP");


                            mtl_log = mtl_log + "Bump map (map_Bump): " + texturePath + "\n";
                        }
                        else
                        {
                            Debug.Log("Error loading texture" + texturePath + " to texture 2D");
                            continue;
                        }

                        if (bumpTex != null && (bumpTex.format == TextureFormat.DXT5 || bumpTex.format == TextureFormat.ARGB32))
                        {
                            OBJLoaderHelper.EnableMaterialTransparency(currentMaterial);
                        }

                    }
                    else
                    {
                        Debug.Log("Texture key \"" + texturePath + "\" found in dictionary but texture was not loaded.");
                        mtl_log = mtl_log + "Error: Bump map " + texturePath + " not loaded." + "\n";
                    }
                }
                else
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Bump map texture \"" + texturePath + "\" not found in dictionary");
                    mtl_log = mtl_log + "Error: Bump map " + texturePath + " not loaded." + "\n";
                    continue;
                }
                continue;
            }

            if (splitLine[0] == "bump" || splitLine[0] == "Bump")
            {
                var texturePath = splitLine[splitLine.Length - 1];
                if (texturePath == null)
                {
                    continue; //invalid args or sth
                }
                if (textures.ContainsKey(texturePath))
                {
                    textures.TryGetValue(texturePath, out var loadedTex);
                    if (loadedTex != null)
                    {
                        var bumpTex = loadedTex as Texture2D;
                        if (bumpTex != null)
                        {
                            currentMaterial.SetTexture("_BumpMap", bumpTex);

                            var bumpScale = GetArgValue(splitLine, "-bm", 1.0f);
                            currentMaterial.SetFloat("_BumpScale", bumpScale);
                            currentMaterial.EnableKeyword("_NORMALMAP");


                            mtl_log = mtl_log + "Bump map (map_Bump): " + texturePath + "\n";
                        }
                        else
                        {
                            Debug.Log("Error loading texture" + texturePath + " to texture 2D");
                            continue;
                        }

                        if (bumpTex != null && (bumpTex.format == TextureFormat.DXT5 || bumpTex.format == TextureFormat.ARGB32))
                        {
                            OBJLoaderHelper.EnableMaterialTransparency(currentMaterial);
                        }

                    }
                }


                for (var i = 0; i < splitLine.Length; i++)
                {
                    if (splitLine[i].Contains("-bm") && i + 1 < splitLine.Length)
                    {
                        currentMaterial.SetFloat("_BumpMultiplier", OBJLoaderHelper.FastFloatParse(splitLine[i + 1]));
                    }
                }

            }

            //alpha clipping
            if (splitLine[0] == "map_d" || splitLine[0] == "map_D")
            {
                var texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                if (texturePath == null)
                {
                    continue; //invalid args or sth
                }

                if (textures.ContainsKey(texturePath))
                {
                    textures.TryGetValue(texturePath, out var loadedTex);
                    if (loadedTex != null)
                    {
                        var alphaTex = loadedTex as Texture2D;
                        if (alphaTex != null)
                        {
                            currentMaterial.SetTexture("_AlphaMap", alphaTex);
                        }
                        else
                        {
                            Debug.Log("Error loading texture" + texturePath + " to texture 2D");
                            continue;
                        }

                        //if (alphaTex != null && (alphaTex.format == TextureFormat.DXT5 || alphaTex.format == TextureFormat.ARGB32))
                        //{
                        //    OBJLoaderHelper.EnableMaterialTransparency(currentMaterial);
                        //}

                    }
                    else
                    {
                        Debug.Log("Texture key \"" + texturePath + "\" found in dictionary but texture was not loaded.");
                    }
                }
                else
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Alpha texture \"" + texturePath + "\" not found in dictionary");
                    continue;
                }
                continue;
            }
            //specular color
            if (splitLine[0] == "Ks" || splitLine[0] == "ks")
            {
                var specularColor = OBJLoaderHelper.ColorFromStrArray(splitLine);

                if (specularColor != null)
                {
                    currentMaterial.SetColor("_SpecColor", specularColor);

                    mtl_log = mtl_log + "Specular color (Ka): " + $"{specularColor.r} {specularColor.g} {specularColor.b} {specularColor.a}" + "\n";
                }

                continue;
            }

            //emission color
            if (splitLine[0] == "Ka" || splitLine[0] == "ka")
            {
                var emissionColor = OBJLoaderHelper.ColorFromStrArray(splitLine, 0.05f);

                if (emissionColor != null)
                {
                    currentMaterial.EnableKeyword("_EmissionColor");
                    currentMaterial.SetColor("_EmissionColor", emissionColor);
                    mtl_log = mtl_log + "Emission color (Ka): " + $"{emissionColor.r} {emissionColor.g} {emissionColor.b} {emissionColor.a}" + "\n";
                }

                continue;
            }

            //ambient color map/emission map 
            if (splitLine[0] == "map_Ka" || splitLine[0] == "map_ka")
            {
                var texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                if (texturePath == null)
                {
                    continue; //invalid args or sth
                }
                else
                {

                }

                if (textures.ContainsKey(texturePath))
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Looking for texture \"" + texturePath + "\" in dictionary");
                    textures.TryGetValue(texturePath, out var loadedTex);

                    if (loadedTex != null)
                    {
                        var emissionTex = loadedTex as Texture2D;
                        if (emissionTex != null)
                        {
                            currentMaterial.SetTexture("_EmissionMap", emissionTex);
                            mtl_log = mtl_log + "Ambient Color/Emission Map (map_Ka): " + texturePath + "\n";

                        }
                        else
                        {
                            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Error loading texture" + texturePath + " to texture 2D");
                            mtl_log = mtl_log + "Error:  Emission map " + texturePath + " not loaded." + "\n";
                            continue;
                        }

                        if (emissionTex != null && (emissionTex.format == TextureFormat.DXT5 || emissionTex.format == TextureFormat.ARGB32))
                        {
                            OBJLoaderHelper.EnableMaterialTransparency(currentMaterial);
                        }

                    }
                    else
                    {
                        Debug.Log("Texture key \"" + texturePath + "\" found in dictionary but texture was not loaded.");
                        mtl_log = mtl_log + "Error:  Emission map " + texturePath + " not loaded." + "\n";
                    }
                }
                else
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Texture \"" + texturePath + "\" not found in dictionary");
                    mtl_log = mtl_log + "Error:  Emission map " + texturePath + " not loaded." + "\n";
                    continue;
                }
            }

            //specular map
            if (splitLine[0] == "map_Ks" || splitLine[0] == "map_ks")
            {
                var texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                if (texturePath == null)
                {
                    continue; //invalid args or sth
                }
                else
                {

                }

                if (textures.ContainsKey(texturePath))
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Looking for texture \"" + texturePath + "\" in dictionary");
                    textures.TryGetValue(texturePath, out var loadedTex);

                    if (loadedTex != null)
                    {
                        var specularMap = loadedTex as Texture2D;
                        if (specularMap != null)
                        {
                            currentMaterial.SetTexture("_SpecularMap", specularMap);
                            mtl_log = mtl_log + "Specular Map (map_Ks): " + texturePath + "\n";

                        }
                        else
                        {
                            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Error loading texture" + texturePath + " to texture 2D");
                            mtl_log = mtl_log + "Error:  specular map " + texturePath + " not loaded." + "\n";
                            continue;
                        }

                        if (specularMap != null && (specularMap.format == TextureFormat.DXT5 || specularMap.format == TextureFormat.ARGB32))
                        {
                            OBJLoaderHelper.EnableMaterialTransparency(currentMaterial);
                        }

                    }
                    else
                    {
                        Debug.Log("Texture key \"" + texturePath + "\" found in dictionary but texture was not loaded.");
                        mtl_log = mtl_log + "Error:  specular map " + texturePath + " not loaded." + "\n";
                    }
                }
                else
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Texture \"" + texturePath + "\" not found in dictionary");
                    mtl_log = mtl_log + "Error:  specular map " + texturePath + " not loaded." + "\n";
                    continue;
                }
            }

            // alpha
            if (splitLine[0] == "d" || splitLine[0] == "D" || splitLine[0] == "Tr" || splitLine[0] == "tr")
            {


                //prefer D values over Tr values as if both present, D more likely to be correct.
                if (splitLine[0] == "Tr" || splitLine[0] == "tr" && dValueAssigned)
                {

                }
                else
                {
                    var visibility = OBJLoaderHelper.FastFloatParse(splitLine[1]);

                    //tr statement is just d inverted
                    if (splitLine[0] == "Tr" || splitLine[0] == "tr")
                    {
                        visibility = 1f - visibility;
                    }
                    else
                    {
                        //if d value toggle bool that D has been assigned so future Tr does not overwrite it.
                        dValueAssigned = true;
                    }


                    //F_REMOVE
                    if (visibility == 0)
                    {
                        if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning("VISIBILITY IN " + currentMaterial.name + ": " + visibility);
                    }

                    if (visibility < (1f - Mathf.Epsilon))
                    {
                        if (AnythingWorld.Utilities.MaterialCacheUtil.
        MaterialCacheDict.TryGetValue("SIMPLE_LIT_TRANSPARENT", out var template))
                        {
                            var newMat = new Material(template);
                            newMat.name = currentMaterial.name;
                            mtlDict[currentMaterial.name] = newMat;
                            currentMaterial = newMat;
                        }


                        var currentColor = currentMaterial.GetColor("_BaseColor");
                        currentColor.a = visibility;
                        currentMaterial.SetColor("_BaseColor", currentColor);
                        mtl_log = mtl_log + "Visibility (tr/d): " + visibility.ToString() + "\n";
                        mtl_log = mtl_log + "Material tranparency: enabled" + "\n";
                    }
                    else
                    {
                        mtl_log = mtl_log + "Visibility (tr/d): " + visibility.ToString() + "\n";
                        mtl_log = mtl_log + "Material tranparency: disabled" + "\n";
                    }

                }


            }

            //glossiness
            if (splitLine[0] == "Ns" || splitLine[0] == "ns")
            {
                var glossiness = OBJLoaderHelper.FastFloatParse(splitLine[1]);
                glossiness = (glossiness / 1000f);
                currentMaterial.SetFloat("_Glossiness", glossiness);
                mtl_log = mtl_log + "Glossiness (ns)" + glossiness.ToString() + "\n";
            }
        }
        reader.Close();



        //F_REMOVE
        if (AnythingSettings.Instance.showDebugMessages) Debug.Log(mtl_log);
        mtl_log = "";

        if (AnythingSettings.Instance.showDebugMessages) Debug.Log(mtl_file);
        mtl_file = "";


        return mtlDict;
    }
}
