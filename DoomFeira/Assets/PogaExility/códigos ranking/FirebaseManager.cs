// FirebaseManager.cs
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    private DatabaseReference dbReference;

    public Task InitializationTask { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializationTask = FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Exception != null)
            {
                Debug.LogError($"Falha ao inicializar Firebase: {task.Exception}");
                return;
            }
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase inicializado com sucesso.");
        });
    }

    // --- A BOA FOI FEITA AQUI ---
    public Task<List<ScoreEntry>> GetTopScores()
    {
        if (dbReference == null) return Task.FromResult(new List<ScoreEntry>());

        // --- INÍCIO DA ADIÇÃO ---
        // Esta linha diz ao Firebase SDK para garantir que os dados locais
        // para o caminho "scores" estejam sincronizados com o servidor.
        // Isso ajuda a evitar que ele retorne dados de um cache antigo.
        dbReference.Child("scores").KeepSynced(true);
        // --- FIM DA ADIÇÃO ---

        return dbReference.Child("scores").OrderByChild("score").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Falha ao buscar pontuações: " + task.Exception);
                return new List<ScoreEntry>();
            }

            DataSnapshot snapshot = task.Result;
            var topScores = new List<ScoreEntry>();
            foreach (var childSnapshot in snapshot.Children)
            {
                topScores.Add(JsonUtility.FromJson<ScoreEntry>(childSnapshot.GetRawJsonValue()));
            }

            topScores.Reverse();
            return topScores;
        });
    }

    public Task AddScore(string name, int score)
    {
        if (dbReference == null) return Task.CompletedTask;
        ScoreEntry newEntry = new ScoreEntry(name, score);
        string json = JsonUtility.ToJson(newEntry);
        return dbReference.Child("scores").Push().SetRawJsonValueAsync(json);
    }
}