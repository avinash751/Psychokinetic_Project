////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;
using static FronkonGames.Artistic.Sharpen.Inspector;

namespace FronkonGames.Artistic.Sharpen.Editor
{
  /// <summary> Spice Up Sharpen inspector. </summary>
  [CustomPropertyDrawer(typeof(Sharpen.Settings))]
  public class SharpenFeatureSettingsDrawer : Drawer
  {
    private Sharpen.Settings settings;

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<Sharpen.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 0.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // Sharpen.
      /////////////////////////////////////////////////
      Separator();

      settings.algorithm = (Sharpen.Algorithm)EnumPopup("Algorithm", "Algorithm used.", settings.algorithm, Sharpen.Algorithm.ContrastAdaptive);

      if (settings.algorithm == Sharpen.Algorithm.Luma)
      {
        IndentLevel++;
        settings.lumaPattern = (Sharpen.LumaPattern)EnumPopup("Pattern", "Blur patterns used with the Luma algorithm.", settings.lumaPattern, Sharpen.LumaPattern.Normal);
        settings.sharpClamp = Slider("Limit", "Limits maximum amount of sharpening a pixel receives [0, 1]. Default 0.035.", settings.sharpClamp, 0.0f, 1.0f, 0.035f);
        IndentLevel--;
      }

      settings.sharpness = Slider("Sharpness", "Strength of the sharpening [0, 1]. Default 1.", settings.sharpness, 0.0f, 1.0f, 1.0f);
      settings.offsetBias = Slider("Offset bias", "Adjusts the radius of the sampling pattern [0, 6]. Default 1.", settings.offsetBias, 0.0f, 6.0f, 1.0f);

      settings.vibrance = Slider("Vibrance", "Color vibrance [0, 2]. Default 0.", settings.vibrance, 0.0f, 2.0f, 0.0f);

      settings.debugView = Toggle("Debug view", "Activate it to see the changing areas of the image. Only available in the Editor.", settings.debugView);

      /////////////////////////////////////////////////
      // Color.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Color") == true)
      {
        IndentLevel++;

        settings.brightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.brightness, -1.0f, 1.0f, 0.0f);
        settings.contrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.contrast, 0.0f, 10.0f, 1.0f);
        settings.gamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.gamma, 0.01f, 10.0f, 1.0f);
        settings.hue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.hue, 0.0f, 1.0f, 0.0f);
        settings.saturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.saturation, 0.0f, 2.0f, 1.0f);

        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Advanced.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Advanced") == true)
      {
        IndentLevel++;

        settings.affectSceneView = Toggle("Affect the Scene View?", "Does it affect the Scene View?", settings.affectSceneView);
        settings.filterMode = (FilterMode)EnumPopup("Filter mode", "Filter mode. Default Bilinear.", settings.filterMode, FilterMode.Bilinear);
        settings.whenToInsert = (UnityEngine.Rendering.Universal.RenderPassEvent)EnumPopup("RenderPass event",
          "Render pass injection. Default BeforeRenderingPostProcessing.",
          settings.whenToInsert,
          UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPostProcessing);
        settings.ignoreDeltaTimeScale = Toggle("Ignore delta time scale", "Set to true to ignore delta time scaling", settings.ignoreDeltaTimeScale);
        settings.enableProfiling = Toggle("Enable profiling", "Enable render pass profiling", settings.enableProfiling);

        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Misc.
      /////////////////////////////////////////////////
      Separator();

      BeginHorizontal();
      {
        if (MiniButton("documentation", "Online documentation") == true)
          Application.OpenURL(Constants.Support.Documentation);

        if (MiniButton("support", "Do you have any problem or suggestion?") == true)
          SupportWindow.ShowWindow();

        if (EditorPrefs.GetBool($"{Constants.Asset.AssemblyName}.Review") == false)
        {
          Separator();

          if (MiniButton("write a review <color=#800000>❤️</color>", "Write a review, thanks!") == true)
          {
            Application.OpenURL(Constants.Support.Store);

            EditorPrefs.SetBool($"{Constants.Asset.AssemblyName}.Review", true);
          }
        }

        FlexibleSpace();

        if (Button("Reset") == true)
          settings.ResetDefaultValues();
      }
      EndHorizontal();
    }
  }
}
