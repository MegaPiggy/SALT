using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class ObjectCopier
{
    public static T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
            Debug.Log((object)"The type must be serializable.");
        if (object.ReferenceEquals((object)source, (object)null))
            return default(T);
        IFormatter formatter = (IFormatter)new BinaryFormatter();
        SurrogateSelector surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), (ISerializationSurrogate)new Vector3Surrogate());
        formatter.SurrogateSelector = (ISurrogateSelector)surrogateSelector;
        Stream serializationStream = (Stream)new MemoryStream();
        using (serializationStream)
        {
            formatter.Serialize(serializationStream, (object)source);
            serializationStream.Seek(0L, SeekOrigin.Begin);
            return (T)formatter.Deserialize(serializationStream);
        }
    }
}
