using itedu_assitant.Model.ForvView;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using JsonOd = System.Text.Json;


namespace itedu_assitant.forsave.Serializer
{
    public class QueueSer
    {
        public static string _path;
        public QueueSer(string? path = null)
        {
            _path = path == null ? $"{Directory.GetCurrentDirectory()}\\Extensions\\Contact_is\\Serializer\\СontactQueue.json" : path;
        }

        public void Serialize(object? queueobject)
        {
            var isval = JsonOd.JsonSerializer.Serialize(queueobject);
            File.WriteAllText(_path, JsonConvert.SerializeObject(queueobject));
        }

        public dynamic DeSerialize<T>()
        {
            if (File.Exists(_path))
                return (T)JsonConvert.DeserializeObject<T>(File.ReadAllText(_path));
            return false;
        }

        public void DataSerialize(object? isclass)
        {
            FileStream writer = new FileStream(_path, FileMode.OpenOrCreate);
            DataContractSerializer dcs = new DataContractSerializer(typeof(Queue<string>));
            dcs.WriteObject(writer, isclass);
            writer.Close();
        }

        public object? DataDeSerialize()
        {
            var val = File.Exists(_path);
            if (val){
                var s = new FileStream(_path, FileMode.Open);
                var ischatval = new DataContractSerializer(typeof(Queue<string>)).ReadObject(s);
                var isstringval = (Queue<string>)ischatval;
                s.Close();
                return isstringval;
            }
            return false;
        }
    }
}
