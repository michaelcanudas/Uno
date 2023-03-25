using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Buffers.Binary;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace Uno;

public abstract class Packet
{
    public static Encoding Encoding { get; } = Encoding.UTF8;

    public Packet()
    {

    }

    /// <summary>
    /// Reads the contents a packet from the provided stream and returns that new packet.
    /// </summary>
    public static Packet Deserialize(Stream stream)
    {
        Debug.Assert(stream.CanRead);

        // read name length
        Span<byte> lengthBytes = stackalloc byte[sizeof(int)];
        stream.Read(lengthBytes);
        
        int nameLength = BitConverter.ToInt32(lengthBytes);

        byte[] nameBytes = new byte[nameLength];
        stream.Read(nameBytes);

        string name = new(Encoding.GetChars(nameBytes));
        Type packetType = Type.GetType(name) ?? throw new Exception("Unknown packet type ???");

        Packet packet = Activator.CreateInstance(packetType) as Packet ?? throw new Exception("Packet type is not a packet ???");

        packet.DeserializeData(stream);

        return packet;
    }

    /// <summary>
    /// Writes the contents of this packet to the provided stream.
    /// </summary>
    public void Serialize(Stream stream)
    {
        Debug.Assert(stream.CanWrite);

        Type packetType = this.GetType();
        string name = packetType.AssemblyQualifiedName ?? throw new("Packet has no type name ???");

        byte[] bytes = Encoding.GetBytes(name);
        byte[] lengthBytes = BitConverter.GetBytes(bytes.Length);

        stream.Write(lengthBytes);
        stream.Write(bytes);

        SerializeData(stream);
    }

    // recursively serializes this packet to a stream.
    protected virtual void SerializeData(Stream stream)
    {
        SerializeFields(new(stream), this);

        static void SerializeFields(BinaryWriter writer, object obj)
        {
            Type writerType = typeof(BinaryWriter);

            const BindingFlags AllFields = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo[] fields = obj.GetType().GetFields(AllFields);
            
            foreach (var field in fields)
            {
                Type fieldType = field.FieldType;
                object value = field.GetValue(obj) ?? throw new($"Cannot serialize null value in field: {field}");

                // if the field is an enum we use its base type
                fieldType = fieldType.IsEnum ? fieldType.UnderlyingSystemType : fieldType;

                MethodInfo? writerMethod = writerType.GetMethod(nameof(BinaryWriter.Write), new[] { fieldType });

                if (writerMethod is not null)
                {
                    writerMethod.Invoke(writer, new[] { value });
                    continue;
                }

                if (fieldType.IsValueType)
                {
                    SerializeFields(writer, value);
                    continue;
                }

                throw new($"Invalid packet field '{field}': Packet fields may only be primitives, strings, enums, or a structs of those types.");
            }
        }
    }


    protected virtual void DeserializeData(Stream stream)
    {
        DeserializeFields(new(stream), this);

        static void DeserializeFields(BinaryReader reader, object obj)
        {
            Type readerType = typeof(BinaryReader);

            const BindingFlags AllFields = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo[] fields = obj.GetType().GetFields(AllFields);

            foreach (var field in fields)
            {
                Type fieldType = field.FieldType;

                // if the field is an enum we use its base type
                fieldType = fieldType.IsEnum ? fieldType.UnderlyingSystemType : fieldType;

                MethodInfo? readerMethod = readerType.GetMethod("Read" + fieldType.Name);

                if (readerMethod is not null)
                {
                    field.SetValue(obj, readerMethod.Invoke(reader, Array.Empty<Type>()));
                    continue;
                }

                if (fieldType.IsValueType)
                {
                    DeserializeFields(reader, Activator.CreateInstance(fieldType)!);
                    continue;
                }

                throw new($"Invalid packet field '{field}': Packet fields may only be primitives, strings, enums, or a structs of those types.");
            }
        }
    }
}