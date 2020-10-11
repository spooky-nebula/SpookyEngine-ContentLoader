using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ContentLoader.Core
{
    public struct AnimatedSpriteList : IContentList<AnimatedSprite>, ITextureList
    {
        public List<AnimatedSprite> List { get; set; }
        public Point RectangleSize { get; set; }

        public override string ToString()
        {
            string result = "AnimatedSpriteList:";
            foreach (AnimatedSprite sprite in List)
            {
                result += sprite.ToString() + ",";
            }
            result = result.Remove(result.Length - 1, 1);
            result += "RectangleSize:" + RectangleSize.ToString();
            return result;
        }
    }

    public struct AnimatedSprite
    {
        public int id;
        public string name;
        public int duration;
        public int frames;

        public override string ToString()
        {
            string result = "{id:" + id + ",name:" + name + ",duration:" + duration + ",frames:" + frames + "}";
            return result;
        }
    }
}
