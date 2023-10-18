using Bell.Data;
using Bell.Utils;

namespace Bell;

public class Folding
{
    public int Start;
    public int End;
    
    public bool Folded;
}

public partial class TextBox
{
    public List<Folding> FoldingList => FoldingListCache.Get();
    public readonly Cache<List<Folding>> FoldingListCache;

    private List<Folding> UpdateFoldingList(List<Folding> foldingList)
    {
        foldingList.Clear();

        Dictionary<int, Stack<int>> foldingStacks = new();
        for (int i = 0; i < Language.Foldings.Count; i++)
        {
            foldingStacks.TryAdd(i, new Stack<int>());
            foldingStacks[i].Clear();
        }
        
        foreach (Line line in Lines)
        {
            for (int i = 0; i < Language.Foldings.Count; i++)
            {
                var startFolding = Language.Foldings[i].Item1;
                var endFolding = Language.Foldings[i].Item2;

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
                int end = Lines.Count - 1;
                AddFolding(foldingList, start, end);
            }
        }
        
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