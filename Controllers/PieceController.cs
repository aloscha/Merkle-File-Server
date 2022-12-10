using MerkleFileServer.Dtos;
using MerkleFileServer.Managers;
using Microsoft.AspNetCore.Mvc;

namespace MerkleFileServer.Controllers
{
    [ApiController]
    [Route("piece")]
    public class PieceController : ControllerBase
    {
        private readonly ITreeManager treeManager;
        public PieceController(
            ITreeManager treeManager
            )
        {
            this.treeManager = treeManager;
        }

        [HttpGet]
        [Route("{hashId}/{pieceIndex}")]
        public IActionResult Get(string hashId, int pieceIndex)
        {
            var response = new PieceResponseDto();
            var piece = treeManager.GetPiece(hashId, pieceIndex);

            if(piece != null)
            {
                response.Content = piece.Content;
                response.Proof = piece.Proof;
            }

            return Ok(response);
        }
    }
}
