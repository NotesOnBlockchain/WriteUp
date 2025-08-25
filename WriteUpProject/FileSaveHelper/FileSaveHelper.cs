using Avalonia.Platform.Storage;
using NBitcoin;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WriteUpProject.FileSaveHelper
{
    public static class FileSaveHelper
    {
        private static FilePickerFileType Psbt { get; } = new("PSBT files")
        {
            Patterns = new[] { "*.psbt" },
            MimeTypes = new[] { "*/*" }
        };

        public static async Task ExportTransactionAsBinary(IStorageProvider storageProvider, PSBT transaction)
        {
            var psbtFileExtension = "psbt";
            var SaveDialogTitle = "Export transaction";

            var file = await SaveFileAsync(SaveDialogTitle, [psbtFileExtension], storageProvider, "MessageForTheBlockchain");

            var filePath = file.Path.LocalPath;

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var ext = Path.GetExtension(filePath);
                if (string.IsNullOrWhiteSpace(ext))
                {
                    filePath = $"{filePath}.{psbtFileExtension}";
                }
                await File.WriteAllBytesAsync(filePath, transaction.ToBytes());
            }
        }

        public static async Task<IStorageFile?> SaveFileAsync(string title, string[] filterExtTypes, IStorageProvider storageProvider,  string? initialFileName = null, string? directory = null)
        {
            if (storageProvider is null)
            {
                return null;
            }

            var suggestedStartLocation = await GetSuggestedStartLocationAsync(directory, storageProvider);

            return await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                FileTypeChoices = [Psbt],
                SuggestedFileName = initialFileName,
                DefaultExtension = filterExtTypes.FirstOrDefault(),
                SuggestedStartLocation = suggestedStartLocation,
                ShowOverwritePrompt = true
            });
        }

        private static async Task<IStorageFolder?> GetSuggestedStartLocationAsync(string? directory, IStorageProvider storageProvider)
        {
            if (directory is not null)
            {
                return await storageProvider.TryGetFolderFromPathAsync(directory);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return await storageProvider.TryGetFolderFromPathAsync(Path.Combine("/media", Environment.UserName));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return await storageProvider.TryGetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            }

            return null;
        }

    }
}
