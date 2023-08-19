using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityUtilities.Singleton;

namespace UnityUtilities.Persistence
{
    public abstract class DataPersistenceManager<T> : SingletonMonoBehaviour<DataPersistenceManager<T>> where T : class
    {
        [SerializeField] protected string _fileName = "data";
        [SerializeField] protected string _parentDirectoryName = "Saves";
        [SerializeField] protected bool _useEncryption = false;


        T _gameData;
        FileDataHandler<T> _fileDataHandler;
        protected override void Awake()
        {
            base.Awake();
            _gameData = CreateEmptyGameData();
            _fileDataHandler = new FileDataHandler<T>(Path.Combine(Application.persistentDataPath, _parentDirectoryName), _fileName, _useEncryption);
        }

        protected abstract T CreateEmptyGameData();

        public bool SaveGame()
        {
            // write important stuff
            foreach (var dataPersistenceObj in FindAllDataPersistenceObjects())
            {
                dataPersistenceObj.SaveData(_gameData);
            }

            // Save file
            try
            {
                _fileDataHandler.Save(_gameData);
            }catch(Exception)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> SaveGameAsync()
        {
            return await Task.FromResult(SaveGame());
        }

        public bool LoadGame()
        {
            // load file
            try
            {
                _gameData = _fileDataHandler.Load();
            }catch(Exception)
            {
                _gameData = null;
            }
            // check if error
            if (_gameData == null)
            {
                _gameData = CreateEmptyGameData();
                return false;
            }
            // apply loading
            foreach (var dataPersistenceObj in FindAllDataPersistenceObjects())
            {
                dataPersistenceObj.LoadData(_gameData);
            }
            return true;
        }
        public async Task<bool> LoadGameAsync()
        {
            return await Task.FromResult(LoadGame());
        }


        IEnumerable<IDataPersistence<T>> FindAllDataPersistenceObjects()
        {
           return FindObjectsOfType<MonoBehaviour>(false).OfType<IDataPersistence<T>>();
        }
    }
}
