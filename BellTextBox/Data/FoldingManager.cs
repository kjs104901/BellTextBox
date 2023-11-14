using Bell.Languages;
using Bell.Utils;

namespace Bell.Data;

// Interface
internal partial class FoldingManager
{
    internal static List<Folding> GetFoldingList() => Singleton.TextBox.FoldingManager.FoldingList;
    internal static void SetCacheDirty() => Singleton.TextBox.FoldingManager._foldingListCache.SetDirty();
}

// Implementation
internal partial class FoldingManager
{
    private List<Folding> FoldingList => _foldingListCache.Get();
    private readonly Cache<List<Folding>> _foldingListCache;

    internal FoldingManager()
    {
        _foldingListCache = new Cache<List<Folding>>("Folding List", new List<Folding>(), UpdateFoldingList);
    }

    private List<Folding> UpdateFoldingList(List<Folding> foldingList)
    {
        foldingList.Clear();
        /*
        Dictionary<int, Stack<int>> foldingStacks = new();
        for (int i = 0; i < Singleton.TextBox.Language.Foldings.Count; i++)
        {
            if (foldingStacks.ContainsKey(i) == false)
                foldingStacks.TryAdd(i, new Stack<int>());
            foldingStacks[i].Clear();
        }
        
        foreach (Line line in LineManager.Lines)
        {
            for (int i = 0; i < Singleton.TextBox.Language.Foldings.Count; i++)
            {
                var startFolding = Singleton.TextBox.Language.Foldings[i].Item1;
                var endFolding = Singleton.TextBox.Language.Foldings[i].Item2;

                for (int j = 0; j < line.CountSubstrings(startFolding); j++)
                {
                    foldingStacks[i].Push(line.Index);
                }
                
                for (int j = 0; j < line.CountSubstrings(endFolding); j++)
                {
                    if (foldingStacks[i].TryPop(out int start))
                    {
                        int end = line.Index;
                        AddFolding(foldingList, start, end);
                    }
                }
            }
        }

        foreach (Stack<int> foldingStack in foldingStacks.Values)
        {
            while (foldingStack.TryPop(out int start))
            {
                int end = LineManager.Lines.Count - 1;
                AddFolding(foldingList, start, end);
            }
        }
        */
        return foldingList;
    }

    private static void AddFolding(List<Folding> foldingList, int start, int end)
    {
        if (start < end)
        {
            foldingList.Add(new Folding()
            {
                Start = start,
                End = end,
                Folded = false
            });
        }
    }
}