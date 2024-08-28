using System;
using UnityEngine;

namespace EW.LUTGenerator
{
    internal readonly struct Gradient2D
    {
        private readonly Gradient gradientA;
        private readonly Gradient gradientB;

        public Gradient2D(Gradient _a, Gradient _b)
        {
            gradientA = _a;
            gradientB = _b;
        }
            
        public Color Evaluate(float _x, float _y, BlendMode _blendMode = BlendMode.MULTIPLY)
        {
            Color a = gradientA.Evaluate(_x);
            Color b = gradientB.Evaluate(_x);

            return _blendMode switch
            {
                BlendMode.BLEND => Color.Lerp(a, b, _y),
                BlendMode.ADD => a + b,
                BlendMode.SUBTRACT => a - b,
                BlendMode.MULTIPLY => a * b,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
