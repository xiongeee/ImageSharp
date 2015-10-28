﻿// <copyright file="PixelOperations.cs" company="James South">
// Copyright (c) James South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor
{
    using System;

    /// <summary>
    /// Performs per-pixel operations.
    /// </summary>
    public static class PixelOperations
    {
        /// <summary>
        /// The array of values representing each possible value of color component
        /// converted from sRGB to the linear color space.
        /// </summary>
        private static readonly Lazy<float[]> LinearLut = new Lazy<float[]>(GetLinearBytes);

        /// <summary>
        /// The array of values representing each possible value of color component
        /// converted from linear to the sRGB color space.
        /// </summary>
        private static readonly Lazy<float[]> SrgbLut = new Lazy<float[]>(GetSrgbBytes);

        /// <summary>
        /// The array of bytes representing each possible value of color component
        /// converted from gamma to the linear color space.
        /// </summary>
        private static readonly Lazy<byte[]> LinearGammaBytes = new Lazy<byte[]>(GetLinearGammaBytes);

        /// <summary>
        /// The array of bytes representing each possible value of color component
        /// converted from linear to the gamma color space.
        /// </summary>
        private static readonly Lazy<byte[]> GammaLinearBytes = new Lazy<byte[]>(GetGammaLinearBytes);

        /// <summary>
        /// Converts an pixel from an sRGB color-space to the equivalent linear color-space.
        /// </summary>
        /// <param name="composite">
        /// The <see cref="Bgra"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Bgra"/>.
        /// </returns>
        public static Bgra ToLinear(ColorVector composite)
        {
            // Create only once and lazily.
            // byte[] ramp = LinearGammaBytes.Value;
            float[] ramp = LinearLut.Value;

            // TODO: This just doesn't seem right to me.
            return new ColorVector(ramp[(composite.B * 255).ToByte()], ramp[(composite.G * 255).ToByte()], ramp[(composite.R * 255).ToByte()], composite.A);
        }

        /// <summary>
        /// Converts a pixel from a linear color-space to the equivalent sRGB color-space.
        /// </summary>
        /// <param name="linear">
        /// The <see cref="Bgra"/> to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Bgra"/>.
        /// </returns>
        public static Bgra ToSrgb(ColorVector linear)
        {
            // Create only once and lazily.
            // byte[] ramp = GammaLinearBytes.Value;
            float[] ramp = SrgbLut.Value;

            // TODO: This just doesn't seem right to me.
            return new ColorVector(ramp[(linear.B * 255).ToByte()], ramp[(linear.G * 255).ToByte()], (linear.R * 255).ToByte(), linear.A);
        }

        /// <summary>
        /// Gets an array of bytes representing each possible value of color component
        /// converted from sRGB to the linear color space.
        /// </summary>
        /// <returns>
        /// The <see cref="T:byte[]"/>.
        /// </returns>
        private static float[] GetLinearBytes()
        {
            float[] ramp = new float[256];
            for (int x = 0; x < 256; ++x)
            {
                float val = SrgbToLinear(x / 255f);
                ramp[x] = val;
            }

            return ramp;
        }

        /// <summary>
        /// Gets an array of bytes representing each possible value of color component
        /// converted from linear to the sRGB color space.
        /// </summary>
        /// <returns>
        /// The <see cref="T:byte[]"/>.
        /// </returns>
        private static float[] GetSrgbBytes()
        {
            float[] ramp = new float[256];
            for (int x = 0; x < 256; ++x)
            {
                float val = LinearToSrgb(x / 255f);
                ramp[x] = val;
            }

            return ramp;
        }

        /// <summary>
        /// Gets the correct linear value from an sRGB signal.
        /// <see href="http://www.4p8.com/eric.brasseur/gamma.html#formulas"/>
        /// <see href="http://entropymine.com/imageworsener/srgbformula/"/>
        /// </summary>
        /// <param name="signal">The signal value to convert.</param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        private static float SrgbToLinear(float signal)
        {
            float a = 0.055f;

            if (signal <= 0.04045)
            {
                return signal / 12.92f;
            }

            return (float)Math.Pow((signal + a) / (1 + a), 2.4);
        }

        /// <summary>
        /// Gets the correct sRGB value from an linear signal.
        /// <see href="http://www.4p8.com/eric.brasseur/gamma.html#formulas"/>
        /// <see href="http://entropymine.com/imageworsener/srgbformula/"/>
        /// </summary>
        /// <param name="signal">The signal value to convert.</param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        private static float LinearToSrgb(float signal)
        {
            float a = 0.055f;

            if (signal <= 0.0031308)
            {
                return signal * 12.92f;
            }

            return ((float)((1 + a) * Math.Pow(signal, 1 / 2.4f))) - a;
        }

        /// <summary>
        /// Gets an array of bytes representing each possible value of color component
        /// converted from gamma to the linear color space.
        /// </summary>
        /// <returns>
        /// The <see cref="T:byte[]"/>.
        /// </returns>
        private static byte[] GetLinearGammaBytes()
        {
            byte[] ramp = new byte[256];
            for (int x = 0; x < 256; ++x)
            {
                byte val = (255f * Math.Pow(x / 255f, 2.2)).ToByte();
                ramp[x] = val;
            }

            return ramp;
        }

        /// <summary>
        /// Gets an array of bytes representing each possible value of color component
        /// converted from linear to the gamma color space.
        /// </summary>
        /// <returns>
        /// The <see cref="T:byte[]"/>.
        /// </returns>
        private static byte[] GetGammaLinearBytes()
        {
            byte[] ramp = new byte[256];
            for (int x = 0; x < 256; ++x)
            {
                byte val = (255f * Math.Pow(x / 255f, 1 / 2.2)).ToByte();
                ramp[x] = val;
            }

            return ramp;
        }
    }
}