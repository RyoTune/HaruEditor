using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using CriFsV2Lib;
using CriFsV2Lib.Definitions;
using CriFsV2Lib.Definitions.Structs;
using HaruEditor.Library.GameDiscovery;
using Microsoft.Extensions.Logging;

namespace HaruEditor.Library.FileProviders.P5R;

public class FileProvider : IFileProvider
{
    private readonly ILogger<FileProvider>? _log;
    private readonly Dictionary<string, CpkFile> _files = new(StringComparer.OrdinalIgnoreCase);
    private string? _gameCpkDir;

    public FileProvider(ILogger<FileProvider>? log, GameDiscoverer games)
    {
        _log = log;
        
        if (games.TryGetSteamGameInstall(1687950, out var gameDir)) Initialize(gameDir);
    }

    public bool IsReady { get; private set; }

    public void Initialize(string gameDir)
    {
        if (IsReady) return;
        
        _gameCpkDir = Path.Join(gameDir, "CPK");
        if (!Directory.Exists(_gameCpkDir)) return;

        foreach (var file in Directory.EnumerateFiles(_gameCpkDir, "*.cpk"))
        {
            using var reader = CriFsLib.Instance.CreateCpkReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), true);

            foreach (var cpkFile in reader.GetFiles())
            {
                var fullPath = Path.Join(Path.GetFileName(file), cpkFile.Directory, cpkFile.FileName)
                    .Replace('\\', '/');
                    
                _files[fullPath] = cpkFile;
            }
        }

        IsReady = true;
    }

    public bool TryGetFile(string filePath, [NotNullWhen(true)] out Stream? outStream)
    {
        outStream = null;
        var cpkFileName = filePath.AsSpan(0, filePath.IndexOf('/'));
        var cpkFilePath = Path.Join(_gameCpkDir, cpkFileName);
        
        if (_files.TryGetValue(filePath, out var cpkFile))
        {
            using var reader = CriFsLib.Instance.CreateCpkReader(new FileStream(cpkFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), true);
            using var extractedFile = reader.ExtractFile(cpkFile);
            outStream = new MemoryStream(extractedFile.Count);
            outStream.Write(extractedFile.Span);
            return true;
        }

        _log?.LogError("File not found.\n{file}", filePath);
        return false;
    }

    public bool TryGetFiles(string[] filePaths, [NotNullWhen(true)] out Dictionary<string, Stream>? outStreams)
    {
        outStreams = null;
        
        using var readersDisp = new CompositeDisposable();
        var readers = new Dictionary<string, ICpkReader>(StringComparer.OrdinalIgnoreCase);

        var tempOutStreams = new Dictionary<string, Stream>();
        var tempStreamsDisp = new CompositeDisposable();
        
        foreach (var filePath in filePaths)
        {
            var cpkFileName = filePath.AsSpan(0, filePath.IndexOf('/')).ToString();
            if (!readers.TryGetValue(cpkFileName, out var reader))
            {
                var cpkFilePath = Path.Join(_gameCpkDir, cpkFileName);
                reader = CriFsLib.Instance.CreateCpkReader(new FileStream(cpkFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), true)
                    .DisposeWith(readersDisp);
                readers[cpkFileName] = reader;
            }
        
            if (_files.TryGetValue(filePath, out var cpkFile))
            {
                using var extractedFile = reader.ExtractFile(cpkFile);
                using var ms = new MemoryStream(extractedFile.Count)
                    .DisposeWith(tempStreamsDisp); // Add to comp disp in case we need to clean up later on file fail.
                
                ms.Write(extractedFile.Span);
                tempOutStreams[filePath] = ms;
            }
            else
            {
                _log?.LogError("File not found.\n{file}", filePath);
                tempStreamsDisp.Dispose(); // Clean up prior successful streams.
                return false;
            }
        }

        outStreams = tempOutStreams;
        return true;
    }
}


