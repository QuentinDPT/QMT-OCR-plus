using Svg;
using System.Collections;

namespace QMTGroup.Overlay;

public class GroupOverlay : IOverlay, IList<IOverlay>
{
    public IOverlay this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(IOverlay item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(IOverlay item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(IOverlay[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<IOverlay> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public int IndexOf(IOverlay item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, IOverlay item)
    {
        throw new NotImplementedException();
    }

    public bool Remove(IOverlay item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public SvgGroup ToSvg()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
