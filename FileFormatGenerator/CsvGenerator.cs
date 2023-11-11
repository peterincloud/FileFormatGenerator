using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace Sonnet.GrayBox.Services
{
    /// <summary>
    /// Transforms an IEnumerable of class objects into a CSV MemoryStream by iterating 
    /// through all its property values. Allows for the conversion of one class object 
    /// into another before writing using a user-defined Func delegate
    /// </summary>
    public class CsvGenerator : IFileFormatGenerator
    {
        public Stream WriteStream<T>(IEnumerable<T> objects) where T : class
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);

            writer.WriteLine(WriteRecord<T>(null));

            foreach (T item in objects)
            {
                writer.WriteLine(WriteRecord(item));
            }

            writer.Flush();
            stream.Position = 0;

            return new MemoryStream(stream.ToArray(), false);
        }

        public Stream WriteStreamWithTransform<N, T>(IEnumerable<N> objects, Func<N, T> tx) where T : class
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);

            writer.WriteLine(WriteRecord<T>(null));

            foreach (N item in objects)
            {
                writer.WriteLine(WriteRecord(tx(item)));
            }

            writer.Flush();
            stream.Position = 0;

            return new MemoryStream(stream.ToArray(), false);
        }
        private static string WriteRecord<T>(T? obj) where T : class
        {
            StringBuilder sb = new();
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();

            for (int i = 0, max = properties.Count; i < max; i++)
            {
                if (properties[i]
                    .GetCustomAttributes()
                    .FirstOrDefault(n => n is System.ComponentModel.DescriptionAttribute) is System.ComponentModel.DescriptionAttribute d)
                {
                    sb.Append(obj == null ? $"{d.Description}" : $"{properties[i].GetValue(obj)?.ToString()}");

                    if (i < max)
                        sb.Append(',');
                }
            }

            return sb.ToString();
        }

    }
}
