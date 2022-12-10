using MerkleFileServer.Dtos;
using MerkleFileServer.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MerkleFileServer.Controllers
{
    [ApiController]
    [Route("hashes")]
    public class HashController : ControllerBase
    {
        private readonly ITreeManager treeManager;
        public HashController(
            ITreeManager treeManager
            )
        {
            this.treeManager = treeManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var allHashes = treeManager.GellAllHashes();
            var response = allHashes.Select(e => new HashResponseDto
            {
                Hash = e.Key,
                Pieces = e.Value,
            }).ToArray();

            return Ok(response);
        }
    }
}
