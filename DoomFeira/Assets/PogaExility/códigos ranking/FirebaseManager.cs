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

    // --- ADI��O IMPORTANTE ---
    // Esta tarefa p�blica vai nos dizer quando a inicializa��o estiver completa.
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
        // Atribu�mos o processo de inicializa��o � nossa tarefa p�blica.
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

    public Task<List<ScoreEntry>> GetTopScores()
    {
        // Esta checagem de seguran�a agora � ainda mais robusta.
        if (dbReference == null) return Task.FromResult(new List<ScoreEntry>());

        // A linha do erro era esta. Agora ela s� ser� chamada quando dbReference n�o for null.
        return dbReference.Child("scores").OrderByChild("score").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Falha ao buscar pontua��es: " + task.Exception);
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