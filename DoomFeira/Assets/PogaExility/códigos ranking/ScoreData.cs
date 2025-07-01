// ScoreData.cs
public static class ScoreData
{
    // Guarda a pontuação final do jogador para ser lida na cena de Game Over.
    public static int PlayerScore { get; set; }

    // Uma "flag" para sabermos que acabamos de vir de uma partida.
    public static bool HasNewScore { get; set; }
}