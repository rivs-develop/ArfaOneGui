using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RIVS.ASAK.Core.Tools.Serialization
{
	/// <summary>
	/// Класс сериализуемого объекта.
	/// </summary>
	[Serializable]
	public class SerializableObject
	{
		// крит секция для защиты сереализации
		[NonSerialized]
		protected object SerializingLock = new object();


		#region Cериализация и десериализация

		/// <summary>
		/// Метод вызывается до сериализации объекта.
		/// </summary>
		/// <param name="context">Контекст сериализации.</param>
		[OnSerializing]
		private void OnSerializingInternal(StreamingContext context)
		{
			if (SerializingLock == null)
            {
                return;
            }

            lock (SerializingLock)
			{
				OnSerializing(context);
			}
		}

		/// <summary>
		/// Метод вызывается до сериализации объекта.
		/// </summary>
		/// <param name="context">Контекст сериализации.</param>
		protected virtual void OnSerializing(StreamingContext context)
		{
		}


		/// <summary>
		/// Метод вызывается после сериализации объекта.
		/// </summary>
		/// <param name="context">Контекст сериализации.</param>
		[OnSerialized]
		private void OnSerializedInternal(StreamingContext context)
		{
			if (SerializingLock == null)
            {
                return;
            }

            lock (SerializingLock)
			{
				OnSerialized(context);
			}
		}

		/// <summary>
		/// Метод вызывается после сериализации объекта.
		/// </summary>
		/// <param name="context">Контекст сериализации.</param>
		protected virtual void OnSerialized(StreamingContext context)
		{
		}

		/// <summary>
		/// Метод вызывается до десериализации объекта.
		/// </summary>
		/// <param name="context">Контекст десериализации.</param>
		[OnDeserializing]
		private void OnDeserializingInternal(StreamingContext context)
		{
			if (SerializingLock == null)
            {
                SerializingLock = new object();
            }

            OnDeserializing(context);
		}

		/// <summary>
		/// Метод вызывается до десериализации объекта.
		/// </summary>
		/// <param name="context">Контекст десериализации.</param>
		protected virtual void OnDeserializing(StreamingContext context)
		{
		}

		/// <summary>
		/// Метод вызывается после десериализации объекта.
		/// </summary>
		/// <param name="context">Контекст десериализации.</param>
		[OnDeserialized]
		private void OnDeserializedInternal(StreamingContext context)
		{
			if (SerializingLock == null)
            {
                SerializingLock = new object();
            }

            OnDeserialized(context);
		}

		/// <summary>
		/// Метод вызывается после десериализации объекта.
		/// </summary>
		/// <param name="context">Контекст десериализации.</param>
		protected virtual void OnDeserialized(StreamingContext context)
		{
		}

		/// <summary>
		/// Метод сериализации объекта в массив байт.
		/// </summary>
		/// <param name="obj">Сериализуемый объект.</param>
		/// <returns>Возвращает массив байт.</returns>
		protected byte[] SerializeToArray(object obj)
		{
			if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            byte[] array;
			using (Stream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, obj);
				array = new byte[stream.Length];
				stream.Seek(0, SeekOrigin.Begin);
				stream.Read(array, 0, array.Length);
			}
			return array;
		}

		/// <summary>
		/// Метод десериализации объекта из массива байт.
		/// </summary>
		/// <param name="array">Массив байт.</param>
		/// <returns>Возвращает сериализованный объект.</returns>
		protected object DeserializeFromArray(byte[] array)
		{
            object obj;
			using (Stream stream = new MemoryStream())
			{
				stream.Write(array, 0, array.Length);
				stream.Seek(0, SeekOrigin.Begin);
				var formatter = new BinaryFormatter();
				obj = formatter.Deserialize(stream);
			}
			return obj;
		}

		#endregion

		#region StaticSerializers

        /// <summary>
        /// Метод сериализации объекта в массив байт.
        /// </summary>
        /// <param name="obj">Сериализуемый объект.</param>
        /// <returns>Возвращает массив байт.</returns>
        public static byte[] SerializeToByteArray(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            byte[] array;
            using (Stream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                array = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(array, 0, array.Length);
            }
            return array;
        }

        /// <summary>
        /// Метод десериализации объекта из массива байт.
        /// </summary>
        /// <param name="array">Массив байт.</param>
        /// <returns>Возвращает сериализованный объект.</returns>
        public static object DeserializeFromByteArray(byte[] array)
        {
            object obj;
            using (Stream stream = new MemoryStream())
            {
                stream.Write(array, 0, array.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var formatter = new BinaryFormatter();
                obj = formatter.Deserialize(stream);
            }
            return obj;
        }
		#endregion
	}
}
