using System; // Basic language types used by the program.
using System.IO; // File and stream types.

namespace GarbageCollectionDemo // Logical grouping for this demo.
{
    public class FileManager : IDisposable // Supports deterministic cleanup.
    {
        private FileStream? _fileStream; // Holds the open file stream.
        private bool _disposed = false; // Tracks whether cleanup already ran.

        public void OpenFile(string path) // Opens a file for reading.
        {
            // Create the stream and keep it in memory.
            _fileStream = new FileStream(path, FileMode.Open);
        }

        public void Dispose() // Public cleanup method for callers.
        {
            // Clean up immediately instead of waiting for GC.
            Dispose(true);
            GC.SuppressFinalize(this); // No finalizer needed after cleanup.
        }

        protected virtual void Dispose(bool disposing) // Core cleanup logic.
        {
            if (_disposed) // Already cleaned.
                return;

            if (disposing)
            {
                // Close the file safely.
                _fileStream?.Dispose();
            }

            _disposed = true; // Mark as cleaned.
        }
    }
}
