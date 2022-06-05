// MIT License

// Copyright (c) 2022, Martin Mayr, Sebastian Fragner

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

/**
 * @project DualSense5Lib
 * @file DS5Helpers.cs
 * @author Martin Mayr
 * @date 05.06.2022
 * @brief Converter functions for colors.
 * 
 * DISCLAIMER: This file is a C# port of
 *  https://github.com/Ohjurot/DualSense-Windows,
 *  which provides a C++ API for the DualSense.
 */

namespace DualSense5Library
{
    internal class Helper
    {
        #region Public Methods
        /// <summary>
        ///  Convert from 3-Color RGB normalized float to DualSense5Library.Color
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <returns>The color resulting from the conversion.</returns>
        public static Color ColorR32G32B32Float(float r, float g, float b)
        {
            return new Color((byte)(255.0F * r), (byte)(255.0F*g), (byte)(255.0F*b));
        }

        /// <summary>
        /// Convert from 4-Color RGBA normalized float to DualSense5Library.Color
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">transparency</param>
        /// <returns>The color resulting from the conversion.</returns>
        public static Color ColorR32G32B32A32(float r, float g, float b, float a)
        {
            return new Color((byte)(255.0F * r * a), (byte)(255.0F * g * a), (byte)(255.0F * b * a));
        }

        /// <summary>
        /// Convert from 4-Color RGBA bytes to DualSense5Library.Color
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">transparency</param>
        /// <returns>The color resulting from the conversion.</returns>
        public static Color ColorR8G8B8Uchar(byte r, byte g, byte b, byte a)
        {
            return new Color((byte)(r * (a / 255.0f)), (byte)(g * (a / 255.0f)), (byte)(b * (a / 255.0f)));
        }
        
        /// <summary>
        /// Convert from 4-Color RGBA (bytes for color, float for transparency) to DualSense5Library.Color.
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">transparency</param>
        /// <returns>The color resulting fro the conversion.</returns>
        public static Color ColorR8G8B8UcharA32Float(byte r, byte g, byte b, float a)
        {
            return new Color((byte)(r * a), (byte)(g * a), (byte)(b * a));
        }
        #endregion
    }
}
