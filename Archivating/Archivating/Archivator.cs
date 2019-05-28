using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Archivating
{
    public delegate void LoadProcess(double proc);
    public class Archivator
    {
        private class InfoBox
        {
            public byte[] Keys { get; set; }
            public BitArray[] Snippets { get; set; }
            public int size { get; set; }
        }

        private class InfoBoxRestore
        {
            public byte[] Keys { get; set; }
            public bool[][] Snippets { get; set; }
            public int size { get; set; }
        }

        public event LoadProcess LoadChanged;

        string file;
        int fileSize;

        private string appDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SuperArchivator");

        byte[] memFile;
        byte[] finalFile;

        public Archivator(string f)
        {
            file = f;
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
        }

        public async Task Upload()
        {
            await Task.Run(() =>
            {
                using (FileStream sr = new FileStream(file, FileMode.Open))
                {
                    int length = (int)sr.Length;
                    memFile = new byte[length];
                    int count;
                    int sum = 0;
                    while ((count = sr.Read(memFile, sum, length - sum)) > 0)
                    {
                        sum += count;
                        LoadChanged?.Invoke((double)sum/(double)length);
                    }
                }
            });
        }

        public async Task Archive()
        {
            await Task.Run(() =>
            {
                BitArray storage = new BitArray(memFile);
                storage = storage.Xor(storage);
                StringAnalizis sa = new StringAnalizis(memFile);
                sa.CountLetters();
                var dictionary = sa.Encrypt();


                int offset = 0;
                foreach(var b in memFile)
                {
                    for(int i = 0; i < dictionary[b].Length; i++)
                    {
                        storage.Set(offset, dictionary[b].Get(i));
                        offset++;
                    }

                    LoadChanged?.Invoke((double)offset / (double)(memFile.Length * 8));
                }

                BitArray final = new BitArray(offset + (8 - (offset % 8)));
                for (int i = 0; i < offset; i++)
                    final.Set(i, storage[i]);

                finalFile = new byte[final.Length / 8];
                final.CopyTo(finalFile,0);
                fileSize = offset;

                SaveDictionary(new InfoBox() { Keys = dictionary.Keys.ToArray(), Snippets = dictionary.Values.ToArray(), size = fileSize });
            });
        }

        public async Task DeArchive()
        {
            await Task.Run(async () =>
            {
                BitArray storage = new BitArray(memFile);

                List<byte> list = new List<byte>();
                var dictionary = await LoadDictionary();
                BitArray token = new BitArray(0);

                for(int i = 0; i < fileSize; i++)
                {
                    token.Length++;
                    token.Set(token.Length - 1, storage.Get(i));

                    if(dictionary.Keys.Contains(token))
                    {
                        list.Add(dictionary[token]);
                        token.Length = 0;
                    }
                }

                finalFile = list.ToArray();                
            });
        }

        public async Task SaveFile()
        {
            await Task.Run(() => 
            {
                using (FileStream sr = new FileStream(file + ".arch", FileMode.Create))
                {
                    sr.Write(finalFile, 0, finalFile.Length);
                }
            });
            
        }

        public async Task SaveFileWithoutArch()
        {
            await Task.Run(() =>
            {
                using (FileStream sr = new FileStream(file.Substring(0,file.Length - 5), FileMode.Create))
                {
                    sr.Write(finalFile, 0, finalFile.Length);
                }
            });

        }

        private async void SaveDictionary(InfoBox b)
        {
            await Task.Run(() => 
            {
                string hash;
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    hash = Convert.ToBase64String(sha1.ComputeHash(finalFile));
                }

                using (StreamWriter sr = new StreamWriter(Path.Combine(appDataPath, hash + ".dic")))
                {
                    sr.Write(JsonConvert.SerializeObject(b));
                }
            });
        }

        private async Task<Dictionary<BitArray, byte>> LoadDictionary()
        {
            Dictionary<BitArray, byte> res = new Dictionary<BitArray, byte>();
            await Task.Run(() =>
            {
                string hash;
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    hash = Convert.ToBase64String(sha1.ComputeHash(memFile));
                }

                using (StreamReader sr = new StreamReader(Path.Combine(appDataPath, hash + ".dic")))
                {
                    var info = JsonConvert.DeserializeObject<InfoBoxRestore>(sr.ReadToEnd());
                    fileSize = info.size;
                    var dictionary = new Dictionary<BitArray, byte>();
                    for (int i = 0; i < info.Keys.Length; i++)
                    {
                        dictionary.Add(new BitArray(info.Snippets[i]), info.Keys[i]);
                    }
                    res = dictionary;
                }
            });

            return res;
        }
    }
}
