
namespace MRL.Utils.Serializer
{
    public interface ISerializer
    {
        byte[] Serialize(object data);
        object DeSerialize(byte[] data);
    }
}
