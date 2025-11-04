namespace HaruEditor.Core.Common;

public static class StreamExtensions
{
    public static Stream AlignStream(this Stream stream, int alignment = 16)
    {
        var offset = stream.Position % alignment;
        if (offset == 0) return stream;
        
        var padding = alignment - offset;
        stream.Position += padding;
        return stream;
    }
}