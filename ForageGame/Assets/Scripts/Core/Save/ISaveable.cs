using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TDK.SaveSystem
{
    public interface ISaveable { public void SaveData(ref WorldSaveData data); }
    public interface ILoadable { public void LoadData(WorldSaveData data); }
}