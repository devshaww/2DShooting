
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemyCount = 30;
    public int upgradePoint = 5;   // kill 10 enemies to upgrade the gun for a little while
    public int enemyKilledInAll;
    public int enemyKilled;
    public static GameManager Instance;

    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform parent;
    public Mode mode = Mode.normal;
    public float upgradeCooldown = 10f;   // crazy mode持续时间

    private GameState state = GameState.GamePlaying;
    private int spawned;
    private bool win;

    public event EventHandler OnStateChange;

    public enum Mode
    {
        normal,
        crazy
    }

    private enum GameState {
        GamePlaying,
        GameOver
    }

    public bool IsGameOver() {
        return state == GameState.GameOver;
    }

    public bool GameWin() {
        return win;
    }

    private void Awake()
    {
        Instance = this;
    }

    //private void Update()
    //{
    //    switch (state)
    //    {
    //        case GameState.GamePlaying:
    //            SpawnEnemies();
    //            break;
    //        case GameState.GameOver:
    //            break;
    //    }
    //}

    private IEnumerator SpawnEnemyCoroutine() {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawned >= enemyCount) break;
            Transform spawnPoint = spawnPoints[i];
            for (int j = 0; j < 5; j++)
            {
                spawned += 1;
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                enemy.transform.parent = parent;
                yield return new WaitForSecondsRealtime(0.5f);
                if (spawned >= enemyCount) break;
            }
        }
    }

    private IEnumerator CrazyModeCoroutine() {
        yield return new WaitForSecondsRealtime(upgradeCooldown);
        mode = Mode.normal;
    }

    public void UpdateData() {
        enemyKilledInAll += 1;
        enemyKilled += 1;
        if (enemyKilled == upgradePoint) {
            Debug.Log("Upgrade");
            mode = Mode.crazy;
            enemyKilled = 0;
            SoundManager.Instance.PlayUpgradeSound(Player.Instance.transform.position);
            StartCoroutine(CrazyModeCoroutine());
        }
        if (enemyKilledInAll == enemyCount) {
            SoundManager.Instance.PlayGameWinSound(Player.Instance.transform.position);
            win = true;
            state = GameState.GameOver;
            OnStateChange?.Invoke(this, EventArgs.Empty);
        }
    }

    public void GameOver() {
        win = false;
        state = GameState.GameOver;
        OnStateChange?.Invoke(this, EventArgs.Empty);
    }

    public void SpawnEnemies() {
        StartCoroutine(SpawnEnemyCoroutine());
    }
}
