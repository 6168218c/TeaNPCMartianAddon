using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TeaNPCMartianAddon.Effects
{
    public struct VertexStripInfo:IVertexType
    {
        public Vector2 Position;
        public Color Color;
        public Vector3 TexCoord;
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(
            new VertexElement[]
            {
                new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
                new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
                new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
            });

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;
        public VertexStripInfo(Vector2 position, Color color,Vector3 coords)
        {
            Position = position;
            Color = color;
            TexCoord = coords;
        }
    }
}
