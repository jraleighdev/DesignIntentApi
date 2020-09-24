using Domain.Models.CouldStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICloudFilesService
    {
        public Task<bool> AddFile(CloudFile file);

        public Task<CloudFile> GetFile(Guid Id);

        public Task<IEnumerable<CloudFile>> GetFiles(string find = "");

        public Task<CloudFile> FindFile(string name);

        public Task<CloudFile> UpdateFile(CloudFile file);

    }
}
