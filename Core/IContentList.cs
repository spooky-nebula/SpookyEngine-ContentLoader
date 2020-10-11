﻿using System.Collections.Generic;

 namespace ContentLoader.Core
{
    public interface IContentList<T>
    {
        public List<T> List { get; set; }
    }
}
