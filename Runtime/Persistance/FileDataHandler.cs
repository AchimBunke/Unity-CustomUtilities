using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace UnityUtilities.Persistence
{
    public class FileDataHandler<T> where T : class
    {
        private string _dataDirPath;
        private string _dataFileName;
        private bool _useEncryption = false;
        private readonly string _encryptionKey = "SuperSecretEncryptionKey";
        public string FullPath => Path.Combine(_dataDirPath, _dataFileName);

        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption = false) 
        {
            _dataDirPath = dataDirPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }

        public T Load()
        {
            var path = FullPath;

            // Full Path
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // return default if no file
            if (!File.Exists(path))
                return null;

            // read Data
            string dataToLoad = "";
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamReader writer = new StreamReader(fs))
                {
                    dataToLoad = writer.ReadToEnd();
                }
            }

            // decrypt
            if (_useEncryption)
            {
                dataToLoad = XORDecryptEncrypt(dataToLoad);
            }

            // Deserialize
            T data = JsonConvert.DeserializeObject<T>(dataToLoad);

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Save(T data)
        {
            var path = FullPath;

            // Full Path
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Serialize
            string dataToStore = JsonConvert.SerializeObject(data);

            // encrypt
            if (_useEncryption)
            {
                dataToStore = XORDecryptEncrypt(dataToStore);
            }

            // Write to file
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataToStore);
                }
            }
        }

        
        string XORDecryptEncrypt(string data)
        {
            int keyLength = _encryptionKey.Length;
            var builder = new StringBuilder();
            for(int i = 0; i < data.Length; i++)
            {
                builder.Append((char)data[i] ^ _encryptionKey[i % keyLength]);
            }
            return builder.ToString();
        }
    }
}
