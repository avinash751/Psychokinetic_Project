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

namespace FronkonGames.Artistic.Sharpen
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Sharpen
  {
    /// <summary> Sharpen algorithms used. </summary>
    public enum Algorithm
    {
      /// <summary>
      /// It applies a blurring effect to the original pixel by incorporating the neighboring pixels, and subsequently,
      /// it subtracts this blur to enhance the image's sharpness. This process is performed in the luma channel to
      /// prevent color artifacts and offers the option to restrict the maximum sharpening effect,
      /// thereby reducing or mitigating halo artifacts. This is similar to using Unsharp Mask in Photoshop.
      /// </summary>
      Luma,

      /// <summary>
      /// It dynamically regulates the sharpening intensity on a per-pixel basis to achieve a uniform level of
      /// sharpness throughout the entire image. Regions in the original image that are already sharp receive a milder
      /// sharpening treatment, whereas regions with less inherent detail undergo a more pronounced sharpening process.
      /// This approach results in an enhanced overall visual sharpness with a reduced occurrence of unwanted artifacts.
      /// </summary>
      ContrastAdaptive,
    }

    /// <summary> Blur patterns used with the Luma algorithm. </summary>
    public enum LumaPattern
    {
      /// <summary> Only two texture fetches. Faster but slightly lower quality. </summary>
      Fast,

      /// <summary> Four texture fetches. </summary>
      Normal,

      /// <summary> Four texture fetches, less sensitive to noise but also to fine details. </summary>
      Wider,

      /// <summary> Four texture fetches, diamond-shaped. A slightly more aggresive look.  </summary>
      Pyramid,
    }
  }
}
