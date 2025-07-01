// ScoreEntry.cs

[System.Serializable] // Permite que a Unity entenda e serialize esta classe.
public class ScoreEntry
{
    public string name; // O nome do jogador (3 letras).
    public int score;   // A pontuação do jogador.

    // Construtor vazio. OBRIGATÓRIO para o Firebase/JSON conseguir criar o objeto.
    public ScoreEntry() { }

    // Construtor para facilitar a criação de novos registros no código.
    public ScoreEntry(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}