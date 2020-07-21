namespace MNie.Cache.Serialization
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using ResultType.Extensions;
    using ResultType.Factories;
    using ResultType.Results;

    public static class ByteSerializer
    {
        public static byte[] Serialize<TItem>(TItem obj)
        {
            if(obj == null)
                return new byte[0];

            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        public static IResult<TItem> Deserialize<TItem>(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);

            return obj switch
            {
                TItem i => i.ToSuccess(),
                _ => ResultFactory.CreateFailure<TItem>($"Data is not deserializable to a type {nameof(TItem)}")
            };
        }    
    }
}