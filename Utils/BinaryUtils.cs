using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SALT.Utils
{
    public static class BinaryUtils
    {
        public static void SwapPattern(byte[] bytes, byte[] pattern, byte[] replacement)
        {
            if (pattern.Length != replacement.Length)
                throw new Exception("Mismatched pattern and replacement length");
            for (int index1 = 0; index1 < bytes.Length - pattern.Length; ++index1)
            {
                bool flag = true;
                for (int index2 = 0; index2 < pattern.Length; ++index2)
                {
                    int index3 = index1 + index2;
                    flag = (int)bytes[index3] == (int)pattern[index2];
                }
                if (flag)
                {
                    for (int index2 = 0; index2 < replacement.Length; ++index2)
                    {
                        int index3 = index1 + index2;
                        bytes[index3] = replacement[index2];
                    }
                }
            }
        }

        public static void WriteMeshWithBones(BinaryWriter writer, SkinnedMeshRenderer rend)
        {
            BinaryUtils.WriteArray(writer, (Array)rend.bones, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteTransform(x, (Transform)y)));
            BinaryUtils.WriteMesh(writer, rend.sharedMesh);
        }

        public static void ReadMeshWithBones(BinaryReader reader, SkinnedMeshRenderer rend)
        {
            int num = reader.ReadInt32();
            for (int index = 0; index < num; ++index)
                BinaryUtils.ReadTransform(reader, rend.bones[index]);
            if (!(bool)(UnityEngine.Object)rend.sharedMesh)
                rend.sharedMesh = new Mesh();
            BinaryUtils.ReadMesh(reader, rend.sharedMesh);
        }

        public static void WriteMesh(BinaryWriter writer, Mesh mesh)
        {
            BinaryUtils.WriteArray(writer, (Array)mesh.vertices, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteVector3(x, (Vector3)y)));
            BinaryUtils.WriteArray(writer, (Array)mesh.triangles, (Action<BinaryWriter, object>)((x, y) => x.Write((int)y)));
            BinaryUtils.WriteArray(writer, (Array)mesh.normals, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteVector3(x, (Vector3)y)));
            BinaryUtils.WriteArray(writer, (Array)mesh.colors, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteColor(x, (Color)y)));
            BinaryUtils.WriteArray(writer, (Array)mesh.uv, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteVector2(x, (Vector2)y)));
            BinaryUtils.WriteArray(writer, (Array)mesh.tangents, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteVector4(x, (Vector4)y)));
            BinaryUtils.WriteArray(writer, (Array)mesh.bindposes, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteMatrix4(x, (Matrix4x4)y)));
            BinaryUtils.WriteArray(writer, (Array)mesh.boneWeights, (Action<BinaryWriter, object>)((x, y) => BinaryUtils.WriteBoneWeight(x, (BoneWeight)y)));
        }

        public static void ReadMesh(BinaryReader reader, Mesh mesh)
        {
            mesh.vertices = BinaryUtils.ReadArray<Vector3>(reader, new Func<BinaryReader, Vector3>(BinaryUtils.ReadVector3));
            mesh.triangles = BinaryUtils.ReadArray<int>(reader, (Func<BinaryReader, int>)(x => x.ReadInt32()));
            mesh.normals = BinaryUtils.ReadArray<Vector3>(reader, new Func<BinaryReader, Vector3>(BinaryUtils.ReadVector3));
            mesh.colors = BinaryUtils.ReadArray<Color>(reader, new Func<BinaryReader, Color>(BinaryUtils.ReadColor));
            mesh.uv = BinaryUtils.ReadArray<Vector2>(reader, new Func<BinaryReader, Vector2>(BinaryUtils.ReadVector2));
            mesh.tangents = BinaryUtils.ReadArray<Vector4>(reader, new Func<BinaryReader, Vector4>(BinaryUtils.ReadVector4));
            mesh.bindposes = BinaryUtils.ReadArray<Matrix4x4>(reader, new Func<BinaryReader, Matrix4x4>(BinaryUtils.ReadMatrix4));
            mesh.boneWeights = BinaryUtils.ReadArray<BoneWeight>(reader, new Func<BinaryReader, BoneWeight>(BinaryUtils.ReadBoneWeight));
        }

        public static void WriteBoneWeight(BinaryWriter writer, BoneWeight weight)
        {
            writer.Write(weight.boneIndex0);
            writer.Write(weight.boneIndex1);
            writer.Write(weight.boneIndex2);
            writer.Write(weight.boneIndex3);
            writer.Write(weight.weight0);
            writer.Write(weight.weight1);
            writer.Write(weight.weight2);
            writer.Write(weight.weight3);
        }

        public static BoneWeight ReadBoneWeight(BinaryReader reader) => new BoneWeight()
        {
            boneIndex0 = reader.ReadInt32(),
            boneIndex1 = reader.ReadInt32(),
            boneIndex2 = reader.ReadInt32(),
            boneIndex3 = reader.ReadInt32(),
            weight0 = reader.ReadSingle(),
            weight1 = reader.ReadSingle(),
            weight2 = reader.ReadSingle(),
            weight3 = reader.ReadSingle()
        };

        public static void WriteColor(BinaryWriter writer, Color color)
        {
            writer.Write(color.r);
            writer.Write(color.g);
            writer.Write(color.b);
            writer.Write(color.a);
        }

        public static Color ReadColor(BinaryReader reader) => new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static void WriteTransform(BinaryWriter writer, Transform transform)
        {
            BinaryUtils.WriteVector3(writer, transform.localScale);
            BinaryUtils.WriteVector3(writer, transform.position);
            BinaryUtils.WriteQuaternion(writer, transform.rotation);
        }

        public static void ReadTransform(BinaryReader reader, Transform trans)
        {
            trans.localScale = BinaryUtils.ReadVector3(reader);
            trans.SetPositionAndRotation(BinaryUtils.ReadVector3(reader), BinaryUtils.ReadQuaternion(reader));
        }

        public static void WriteQuaternion(BinaryWriter writer, Quaternion quat)
        {
            writer.Write(quat.x);
            writer.Write(quat.y);
            writer.Write(quat.z);
            writer.Write(quat.w);
        }

        public static Quaternion ReadQuaternion(BinaryReader reader) => new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static void WriteVector3(BinaryWriter writer, Vector3 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
        }

        public static Vector3 ReadVector3(BinaryReader reader) => new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static void WriteList<T>(
          BinaryWriter writer,
          List<T> list,
          Action<BinaryWriter, T> listWriter)
        {
            writer.Write(list.Count);
            foreach (T obj in list)
                listWriter(writer, obj);
        }

        public static void ReadList<T>(
          BinaryReader reader,
          List<T> list,
          Func<BinaryReader, T> listReader)
        {
            list.Clear();
            int num = reader.ReadInt32();
            for (int index = 0; index < num; ++index)
                list.Add(listReader(reader));
        }

        public static void WriteDictionary<K, V>(
          BinaryWriter writer,
          Dictionary<K, V> dict,
          Action<BinaryWriter, K> keyWriter,
          Action<BinaryWriter, V> valueWriter)
        {
            writer.Write(dict.Count);
            foreach (KeyValuePair<K, V> keyValuePair in dict)
            {
                keyWriter(writer, keyValuePair.Key);
                valueWriter(writer, keyValuePair.Value);
            }
        }

        public static void ReadDictionary<K, V>(
          BinaryReader reader,
          Dictionary<K, V> dict,
          Func<BinaryReader, K> keyReader,
          Func<BinaryReader, V> valueReader)
        {
            dict.Clear();
            int num = reader.ReadInt32();
            for (int index = 0; index < num; ++index)
                dict.Add(keyReader(reader), valueReader(reader));
        }

        public static void WriteVector2(BinaryWriter writer, Vector2 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
        }

        public static Vector2 ReadVector2(BinaryReader reader) => new Vector2(reader.ReadSingle(), reader.ReadSingle());

        public static void WriteVector4(BinaryWriter writer, Vector4 vec)
        {
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
            writer.Write(vec.w);
        }

        public static Vector4 ReadVector4(BinaryReader reader) => new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        public static void WriteMatrix4(BinaryWriter writer, Matrix4x4 matrix)
        {
            for (int index = 0; index < 4; ++index)
                BinaryUtils.WriteVector4(writer, matrix.GetColumn(index));
        }

        public static Matrix4x4 ReadMatrix4(BinaryReader reader) => new Matrix4x4(BinaryUtils.ReadVector4(reader), BinaryUtils.ReadVector4(reader), BinaryUtils.ReadVector4(reader), BinaryUtils.ReadVector4(reader));

        public static void WriteArray(
          BinaryWriter writer,
          Array array,
          Action<BinaryWriter, object> writeAction)
        {
            writer.Write(array.Length);
            for (int index = 0; index < array.Length; ++index)
                writeAction(writer, array.GetValue(index));
        }

        public static T[] ReadArray<T>(BinaryReader reader, Func<BinaryReader, T> readAction)
        {
            int length = reader.ReadInt32();
            T[] objArray = new T[length];
            for (int index = 0; index < length; ++index)
                objArray[index] = readAction(reader);
            return objArray;
        }
    }
}
