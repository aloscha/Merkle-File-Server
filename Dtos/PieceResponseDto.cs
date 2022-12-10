namespace MerkleFileServer.Dtos
{
    public class PieceResponseDto
    {
        public string Content { get;set; }
        public string[] Proof { get; set; }
    }
}
