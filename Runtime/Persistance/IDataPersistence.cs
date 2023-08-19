namespace UnityUtilities.Persistence
{
    public interface IDataPersistence<T> where T : class
    {
        void LoadData(T gameData);
        void SaveData(T gameData);
    }
}
