using System.Reflection;
using Echoes;
using HaruEditor.Desktop.Models;

namespace HaruEditor.Desktop.Localization;

public static class ObjectLocalizer
{
    private static readonly Assembly LocAss = typeof(Objects).Assembly;
    private const string ObjLocFile = @"Source\Objects.toml";
    private const string TranslationNotFound = "TRANSLATION NOT FOUND";
    
    public static string GetValue(object? obj)
    {
        if (obj == null) return "null";

        if (obj is ObjectWithId objectWithId)
        {
            var objIdKey = objectWithId.Object.GetType().FullName!.Replace('.', '_');
            var locIdValue = new TranslationUnit(LocAss, ObjLocFile, $"{objIdKey}_{objectWithId.Id}").CurrentValue;
            
            if (locIdValue.StartsWith(TranslationNotFound))
            {
                locIdValue =
                    $"{new TranslationUnit(LocAss, ObjLocFile, objIdKey).CurrentValue} {objectWithId.Id}";
            }
            
            return locIdValue;
        }

        var objKey = $"{obj!.GetType().FullName!.Replace('.', '_')}";
        var locValue = new TranslationUnit(LocAss, ObjLocFile, objKey).CurrentValue;
        if (locValue.StartsWith(TranslationNotFound))
        {
            locValue = objKey;
        }

        return locValue;
    }
}