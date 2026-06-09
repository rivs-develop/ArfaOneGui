using System;
using System.Diagnostics;
using System.IO;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    /// <summary>
    /// Добавление и контроль версионности при кастомной сериализации
    /// </summary>
    public static class VersionableCustomSerializable
    {
        /// <summary>
        /// Сериализация
        /// </summary>
        /// <param name="writer">Писатель (поток для записи)</param>
        /// <param name="dataVersion">Версия данных</param>
        /// <param name="realSerialize">Делегат, производящий реальную сериализацию данных</param>
        static public void CustomSerialize(BinaryWriter writer, byte dataVersion, Action realSerialize)
        {
            const int sizeLen = 4;
            writer.Write((byte)dataVersion);
            var sizePos = writer.BaseStream.Position;
            writer.BaseStream.Seek(sizeLen, SeekOrigin.Current);

            realSerialize.Invoke();

            var endPos = writer.BaseStream.Position;
            var dateSize = (Int32)(endPos - sizePos);
            writer.BaseStream.Seek(-dateSize, SeekOrigin.Current);
            writer.Write(dateSize);
            writer.BaseStream.Seek(dateSize - sizeLen, SeekOrigin.Current);
            Debug.Assert(endPos == writer.BaseStream.Position);
        }

        /// <summary>
        /// Десериализация
        /// </summary>
        /// <param name="reader">Читатель (поток для чтения)</param>
        /// <param name="dataVersion">Версия данных</param>
        /// <param name="realDeserialize">Делегат, производящий реальную десериализацию данных, содержит параметр - вирсию пришедших данных</param>
        static public void CustomDeserialize(BinaryReader reader, byte dataVersion, Action<byte> realDeserialize)
        {
            var version = reader.ReadByte();
            var sizePos = reader.BaseStream.Position;
            var dataSize = reader.ReadInt32();
            var endPos = sizePos + dataSize;

            realDeserialize.Invoke(version);

            if (reader.BaseStream.Position != endPos)
            {
                // проверка на ошибки
                if (version == dataVersion ||               // разный размер при одинаковой версии
                    reader.BaseStream.Position > endPos)    // прочитали больше чем нужно
                {
                    // такого быть не должно!
                    throw new InvalidDataException(String.Format("Ошибка десериализации CustomSerializable: size {0} realSize {1}, Ver {2}", dataSize, reader.BaseStream.Position - sizePos, version));

                }
                // передвигаем позицию чтения вперед (чтение младшей версии более старшей)
                reader.BaseStream.Seek(endPos, SeekOrigin.Begin);
            }
        }

    }
}