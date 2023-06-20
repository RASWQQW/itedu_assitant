using itedu_assitant.Model.ForvView;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace itedu_assitant.forsave.Serializer
{
    public class QueueSer
    {
        public static string _path;
        public QueueSer(string? path = null)
        {
            _path = path == null ? $"{Directory.GetCurrentDirectory()}\\forsave\\Contact_is\\Serializer\\contactQueue.xml" : path;
        }

        public void Serialize(object? isclass)
        {
            FileStream writer = new FileStream(_path, FileMode.OpenOrCreate);
            DataContractSerializer dcs = new DataContractSerializer(typeof(Queue<string>));
            dcs.WriteObject(writer, isclass);
            writer.Close();
        }

        public object? DeSerialize()
        {
            if(Directory.Exists(_path)){
                Stream s = File.Open(_path, FileMode.Open);
                var isstringval = (Queue<string>)new DataContractSerializer(typeof(Queue<string>)).ReadObject(s);
                s.Close();
                return isstringval;
            }
            return false;
        }
    }
}
