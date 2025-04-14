using System.Collections;

namespace QMTGroup.Overlay;

public class GroupOverlay : IOverlay, IList<IOverlay>
{
    private List<IOverlay> _overlays = new();

    public IOverlay this[int index] { get => _overlays[index]; set => _overlays[index] = value; }

    public int Count => _overlays.Count();

    public virtual bool IsReadOnly => true;

    public void Add(IOverlay item) => _overlays.Add(item);

    public void Clear() => _overlays.Clear();

    public bool Contains(IOverlay item) => _overlays.Contains(item);

    public void CopyTo(IOverlay[] array, int arrayIndex) => _overlays.CopyTo(array, arrayIndex);

    public IEnumerator<IOverlay> GetEnumerator() => _overlays.GetEnumerator();

    public int IndexOf(IOverlay item) => _overlays.IndexOf(item);

    public void Insert(int index, IOverlay item) => _overlays.Insert(index, item);

    public bool Remove(IOverlay item) => _overlays.Remove(item);

    public void RemoveAt(int index) => _overlays.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => _overlays.GetEnumerator();
}
