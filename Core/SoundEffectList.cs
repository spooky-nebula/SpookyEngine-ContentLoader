using System.Collections.Generic;

namespace ContentLoader.Core
{
    public struct SoundEffectList : IContentList<Sound>
    {
        public List<Sound> List { get; set; }
    }

    public struct Sound
    {
        public string file;
        public string name;
    }
}