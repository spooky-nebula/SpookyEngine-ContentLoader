using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ContentLoader.Core
{
    public struct SpriteList : IContentList<Sprite>, ITextureList
    {
        public List<Sprite> List { get; set; }
        public RectangleSize RectangleSize { get; set; }

        public override string ToString()
        {
            string result = "SpriteList:";
            foreach (Sprite sprite in List)
            {
                result += sprite.ToString() + ",";
            }
            result = result.Remove(result.Length - 1, 1);
            result += "RectangleSize:" + RectangleSize.ToString();
            return result;
        }
    }

    public struct Sprite
    {
        public int id;
        public string name;

        public override string ToString()
        {
            string result = "{id:" + id + ",name:" + name + "}";
            return result;
        }
    }

    public struct RectangleSize
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            string result = "{X:" + X + ",Y:" + Y + "}";
            return result;
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }
    }
}
