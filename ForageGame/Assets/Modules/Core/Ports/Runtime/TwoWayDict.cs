using System.Collections.Generic;
using System.Linq;

public class TwoWayDict<TSource, TTarget>
{
    private Dictionary<TSource, HashSet<TTarget>> _sourceToTargets = new();
    private Dictionary<TTarget, HashSet<TSource>> _targetToSources = new();

    #region Get

    // Getting functions return copies!!!

    public HashSet<TTarget> GetTargetsBySource(TSource source) => new(_sourceToTargets[source]);
    public HashSet<TSource> GetSourcesByTarget(TTarget target) => new(_targetToSources[target]);

    #endregion

    #region Add

    public void Add(TSource source, TTarget target)
    {
        // Check if the hashset needs to be initialized before editing
        if (!_sourceToTargets.ContainsKey(source) || _sourceToTargets[source] == null) _sourceToTargets[source] = new();
        if (!_targetToSources.ContainsKey(target) || _targetToSources[target] == null) _targetToSources[target] = new();
        _sourceToTargets[source].Add(target);
        _targetToSources[target].Add(source);
    }
    public void AddEntries(TSource source, List<TTarget> targets) { foreach (TTarget target in targets) Add(source, target); }
    public void AddEntries(List<TSource> sources, TTarget target) { foreach (TSource source in sources) Add(source, target); }

    #endregion

    #region Remove

    public void Remove(TSource source, TTarget target)
    {
        if (_sourceToTargets.ContainsKey(source))
        {
            _sourceToTargets[source]?.Remove(target);
            if (_sourceToTargets[source] == null)
                _sourceToTargets.Remove(source);
        }
        if (_targetToSources.ContainsKey(target))
        {
            _targetToSources[target]?.Remove(source);
            if (_targetToSources[target] == null)
                _targetToSources.Remove(target);
        }
    }
    public void RemoveEntries(TSource source, List<TTarget> targets) { foreach (TTarget target in targets) Remove(source, target); }
    public void RemoveEntries(List<TSource> sources, TTarget target) { foreach (TSource source in sources) Remove(source, target); }
    public void RemoveEntriesBySource(TSource source) { RemoveEntries(source, GetTargetsBySource(source).ToList()); }
    public void RemoveEntriesByTarget(TTarget target) { RemoveEntries(GetSourcesByTarget(target).ToList(), target); }

    #endregion

    #region Clear

    public void Clear()
    {
        _sourceToTargets.Clear();
        _targetToSources.Clear();
    }

    #endregion
}