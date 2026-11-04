using Source.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Source.Common
{
    public sealed class GameContext(
        GameGeometry geometry,
        UserInput input)
    {
        public GameGeometry Geometry { get; init; } = geometry;

        public UserInput InputData { get; init; } = input;
    }
}
