using System;
using System.Collections.Generic;
using System.Text;

namespace web2book
{
    public interface IContentSourceList
    {
        void Add(ContentSource cs);
        void ResetBindings();
        int Count { get; }
        ContentSource this[int index] { get; }
        void RemoveAt(int index);
        bool HasSource(ContentSource s);
    }
}
