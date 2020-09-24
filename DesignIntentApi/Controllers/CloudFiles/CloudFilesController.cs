using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models.CouldStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DesignIntentApi.Controllers.CloudFiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudFilesController : ControllerBase
    {
        private readonly ICloudFilesService _cloudFiles;

        public CloudFilesController(ICloudFilesService cloudFiles)
        {
            _cloudFiles = cloudFiles;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CloudFile>> AddFile(CloudFile file)
        {
            file.Id = Guid.NewGuid();

            var newFile = await _cloudFiles.AddFile(file);

            if (!newFile)
            {
                return BadRequest();
            }

            return Ok(newFile);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<CloudFile>> UpdateCloudFile(CloudFile file)
        {
            return await _cloudFiles.UpdateFile(file);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CloudFile>> GetFile(Guid id)
        {
            return await _cloudFiles.GetFile(id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CloudFile>>> GetFiles()
        {
            return Ok(await _cloudFiles.GetFiles());
        }
    }
}
