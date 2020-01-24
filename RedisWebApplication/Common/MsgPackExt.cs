using MessagePack;
using StackExchange.Redis.Extensions.Core;
using System;
using System.IO;
using System.Text;
//using MsgPack.Serialization;

namespace RedisWebApplication.Common
{
 public class MsgPackObjectSerializerExt : ISerializer
  {
    private readonly Encoding encoding;

        static MsgPackObjectSerializerExt()
        {
            //MsgPack.Serialization.MessagePackSerializer. SetDefaultResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);
        }

    public MsgPackObjectSerializerExt(
      Action<MsgPack.Serialization.SerializerRepository> customSerializerRegistrar = null,
      Encoding encoding = null)
    {
      //if (customSerializerRegistrar != null)
        //customSerializerRegistrar(MsgPack.Serialization.SerializerRepository.Default.GetRegisteredSerializers);
      if (encoding != null)
        return;
      this.encoding = Encoding.UTF8;
    }

    public T Deserialize<T>(byte[] serializedObject)
    {
      if (typeof (T) == typeof (string))
        return (T) Convert.ChangeType((object) this.encoding.GetString(serializedObject), typeof (T));
      
        var lz4Options = MessagePack.Resolvers.ContractlessStandardResolver.Options
                    .WithCompression(MessagePackCompression.Lz4BlockArray);

      using (MemoryStream memoryStream = new MemoryStream(serializedObject))
        return MessagePackSerializer.Deserialize<T>(serializedObject, lz4Options);
    }

    public byte[] Serialize(object item)
    {
      if (item is string)
        return this.encoding.GetBytes(item.ToString());
      using (MemoryStream memoryStream = new MemoryStream())
      {
        var lz4Options = MessagePack.Resolvers.ContractlessStandardResolver.Options
                    .WithCompression(MessagePackCompression.Lz4BlockArray);

        MessagePackSerializer.Serialize(memoryStream, item, lz4Options);    
        return memoryStream.ToArray();
      }
    }

    public object Deserialize(byte[] serializedObject)
    {
      return this.Deserialize<object>(serializedObject);
    }
  }
}
