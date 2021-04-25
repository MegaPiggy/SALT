using System;
using System.Runtime.Serialization;
using UnityEngine;

public class Vector3Surrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 vector3 = (Vector3)obj;
        info.AddValue("x", vector3.x);
        info.AddValue("y", vector3.y);
        info.AddValue("z", vector3.z);
    }

    public object SetObjectData(
      object obj,
      SerializationInfo info,
      StreamingContext context,
      ISurrogateSelector selector)
    {
        Vector3 vector3 = (Vector3)obj;
        try
        {
            vector3.x = (float)info.GetDecimal("x");
            vector3.y = (float)info.GetDecimal("y");
            vector3.z = (float)info.GetDecimal("z");
        }
        catch
        {
            Debug.Log((object)"Failed to load vector data, setting to starting pos");
            vector3.x = 88.21f;
            vector3.y = 16.41f;
            vector3.z = -139.86f;
        }
        return (object)vector3;
    }
}
