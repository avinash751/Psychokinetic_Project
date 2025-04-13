using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Color = UnityEngine.Color;
using UnityEditorInternal;


public static class FlatColorMaterialGenerator
{
    public static Material GenerateMaterialWithHexCode(string hexInput, string materialName, string filePath)
    {
        Material newMaterial = GenerateURPMaterial(materialName, filePath);
        ColorUtility.TryParseHtmlString(hexInput, out Color generatedHexColor);

        newMaterial.color = generatedHexColor;
        return newMaterial;
    }

    public static Material GenerateMaterialWithColor(Color color, string materialName, string filePath)
    {
        Material newMaterial = GenerateURPMaterial(materialName, filePath);
        newMaterial.color = color;
        return newMaterial;
    }

    static Material GenerateURPMaterial(string materialName, string filePath)
    {
        Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        string materialPath = filePath + materialName + ".mat";
        AssetDatabase.CreateAsset(newMaterial, materialPath);
        AssetDatabase.SaveAssets();
        Undo.RegisterCreatedObjectUndo(newMaterial, "Created new material");
        return newMaterial;
    }
    public static void SetEmissionForMaterial(Material material, Color emissionColor, float emissionIntensity)
    {
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", emissionColor * emissionIntensity);
    }
}

public class MaterialGeneratorWindow : EditorWindow
{

    private Vector2 minWindowSize = new Vector2(340, 500);
    private Vector2 maxWindowSize = new Vector2(340, 500);
    private Vector2 scrollPosition = Vector2.zero;

    [SerializeField] List<URPFlatColorMaterialProperties> materialPropertiesList = new List<URPFlatColorMaterialProperties>();
    private ReorderableList reorderableList;
    Dictionary<FilePathType, string> filePathTypes = new Dictionary<FilePathType, string>();
    FilePathType selectedFilePathType = 0;

    string customMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/Custom/";
    string flatColorMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/FlatColor/";
    string flatColorEmissiveMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/FlatColorEmmisive/";
    string particleMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/Particle/";
    string UIMaterialsPath = "Assets/PROJECT/ArtAssets/Materials/UI/";

    enum FilePathType
    {
        CustomMaterials,
        FlatColorEmissiveMaterials,
        FlatColorMaterials,
        ParticleMaterials,
        UIMaterials
    }

    enum MaterialType
    {
        Custom,
        FlatColor,
        FlatColorEmissive,
        Particle,
        UI
    }

    [MenuItem("Tools/Material Generator")]
    [MenuItem("Assets/Material Generator")]
    static void OpenMaterialGeneratorWindow()
    {
        MaterialGeneratorWindow window = GetWindow<MaterialGeneratorWindow>("Material Generator");
        window.minSize = window.minWindowSize;
        window.maxSize = window.maxWindowSize;

        window.filePathTypes.Add(FilePathType.CustomMaterials, window.customMaterialsPath);
        window.filePathTypes.Add(FilePathType.FlatColorMaterials, window.flatColorMaterialsPath);
        window.filePathTypes.Add(FilePathType.FlatColorEmissiveMaterials, window.flatColorEmissiveMaterialsPath);
        window.filePathTypes.Add(FilePathType.ParticleMaterials, window.particleMaterialsPath);
        window.filePathTypes.Add(FilePathType.UIMaterials, window.UIMaterialsPath);
        window.Show();
    }

    private void OnEnable()
    {
        materialPropertiesList = new List<URPFlatColorMaterialProperties>();
        reorderableList = new ReorderableList(materialPropertiesList, typeof(URPFlatColorMaterialProperties), true, true, true, true);

    }
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        reorderableList.drawHeaderCallback = (rect) =>
        {
            EditorGUI.LabelField(rect, "Material Properties");
        };

        DrawingMaterialListAndProperties();

        EditorGUILayout.LabelField("File Path Type", EditorStyles.boldLabel);
        EditorStyles.popup.fixedHeight = 25;
        selectedFilePathType = (FilePathType)EditorGUILayout.EnumPopup(selectedFilePathType);

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Generate Materials", GUILayout.Height(30)))
        {
            Debug.Log("Generating Materials");
            foreach (URPFlatColorMaterialProperties materialProperties in materialPropertiesList)
            {

                if (materialProperties.useHexInput)
                {

                    FlatColorMaterialGenerator.GenerateMaterialWithHexCode(materialProperties.hexInput, materialProperties.materialName, filePathTypes[selectedFilePathType]);
                }
                else
                {
                    FlatColorMaterialGenerator.GenerateMaterialWithColor(materialProperties.albedoColor, materialProperties.materialName, filePathTypes[selectedFilePathType]);
                }
            }
        }
        EditorGUILayout.Space(20);
        EditorGUILayout.EndScrollView();
    }

    private void DrawingMaterialListAndProperties()
    {
        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = materialPropertiesList[index];
            element.foldOut = EditorGUI.Foldout(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), element.foldOut, "Material " + index + " Properties");
           
            if (element.foldOut)
            {
                reorderableList.elementHeightCallback = (i) => materialPropertiesList[i].foldOut == element.foldOut ? 200 : reorderableList.elementHeight;
                element.hexInput = EditorGUI.TextField(new Rect(rect.x + 10, rect.y += 20, 250, EditorGUIUtility.singleLineHeight), "HexCode", element.hexInput);
                element.useHexInput = EditorGUI.Toggle(new Rect(rect.x + 10, rect.y += 20, 250, EditorGUIUtility.singleLineHeight), "Use Hex Input", element.useHexInput);
                element.materialName = EditorGUI.TextField(new Rect(rect.x + 10, rect.y += 20, 250, EditorGUIUtility.singleLineHeight), "Material Name", element.materialName);
                element.albedoColor = EditorGUI.ColorField(new Rect(rect.x + 10, rect.y += 20, 250, EditorGUIUtility.singleLineHeight), "Albedo Color", element.albedoColor);
                rect.y += 10;
                element.enableEmission = EditorGUI.Toggle(new Rect(rect.x + 10, rect.y += 20, 250, EditorGUIUtility.singleLineHeight), "Enable Emission", element.enableEmission);
                element.emissionColor = EditorGUI.ColorField(new Rect(rect.x + 10, rect.y += 20, 250, EditorGUIUtility.singleLineHeight), "Emission Color", element.emissionColor);
                element.emissionIntensity = EditorGUI.FloatField(new Rect(rect.x + 10, rect.y += 20, 250, EditorGUIUtility.singleLineHeight), "Emission Intensity", element.emissionIntensity);
            }
          


        };
        reorderableList.DoLayoutList();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }


}


[Serializable]
public class URPFlatColorMaterialProperties
{
    //Common Material Properties
    public string hexInput = "#FFFFFF"; // Default value
    public string materialName = "NewMaterial";
    public Color albedoColor = Color.white;
    public bool useHexInput = true;

    //Material Emission Properties
    public bool enableEmission = false;
    public Color emissionColor = Color.black;
    public float emissionIntensity = 3.0f;

    public bool foldOut;
}

