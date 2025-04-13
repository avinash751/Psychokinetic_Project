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
#pragma once

float _Vibrance;

inline float3 Vibrance(float3 pixel)
{
  float3 _VibranceCoeff = _Vibrance;
  float luma = Luminance(pixel);
  float maxColor = max(pixel.r, max(pixel.g, pixel.b));
  float minColor = min(pixel.r, min(pixel.g, pixel.b));

  float saturation = maxColor - minColor;

  pixel = lerp(luma, pixel, (1.0 + (_VibranceCoeff * (1.0 - (sign(_VibranceCoeff) * saturation)))));

  return pixel;
}
