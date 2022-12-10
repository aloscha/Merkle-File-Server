using System.Collections.Generic;
using System.IO;
using System;

namespace MerkleFileServer.Services
{
    public class FileService : IFileService
    {
        private readonly IAppConfigService appConfigService;
        public FileService(IAppConfigService appConfigService)
        {
            this.appConfigService = appConfigService;
        }
        public IEnumerable<byte[]> ReadChunks(string path)
        {
            if(!File.Exists(path))
                throw new InvalidOperationException("Missing file");

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            long remainBytes = fs.Length;
            int bufferBytes = appConfigService.FileSize;

            while (true)
            {
                var nextBytes = Math.Min(remainBytes, appConfigService.FileSize);
                byte[] filechunk = new byte[nextBytes];
                bufferBytes = (int)nextBytes;

                if ((fs.Read(filechunk, 0, bufferBytes)) > 0)
                {
                    remainBytes -= bufferBytes;
                    yield return filechunk;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
