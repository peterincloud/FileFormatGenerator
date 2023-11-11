using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sonnet.GrayBox.Services
{
    public interface IFileFormatGenerator
    {
        Stream WriteStream<T>(IEnumerable<T> obj) where T : class;
        Stream WriteStreamWithTransform<N, T>(IEnumerable<N> obj, Func<N, T> transformFx) where T : class;
    }
}
