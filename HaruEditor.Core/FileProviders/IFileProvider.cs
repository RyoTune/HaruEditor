using System.Diagnostics.CodeAnalysis;

namespace HaruEditor.Core.FileProviders;

public interface IFileProvider
{
    public bool IsReady { get; }
    public void Initialize(string gameDir);
    public bool TryGetFile(string filePath, [NotNullWhen(true)] out Stream? outStream);
    public bool TryGetFiles(string[] filePaths, [NotNullWhen(true)] out Dictionary<string, Stream>? outStreams);
}