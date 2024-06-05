using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 移動先のシーン名
    public string targetScene;

    // プレイヤーの出現位置
    public Vector3 spawnPosition;

    // ワープ機能が有効かどうかを示すフラグ
    bool canWarp = false;

    // プレイヤーの初期位置
    Vector3 playerInitialPosition;

    // プレイヤーオブジェクト
    GameObject player;

    void Start()
    {
        // プレイヤーオブジェクトを探して初期位置を取得
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInitialPosition = player.transform.position;
        }
    }

    void Update()
    {
        if (player != null)
        {
            // プレイヤーが初期位置から1f以上離れているか確認
            if (Vector3.Distance(player.transform.position, playerInitialPosition) >= 1f)
            {
                canWarp = true;
            }
        }
    }

    // トリガーに触れた際に呼び出される関数
    void OnTriggerEnter2D(Collider2D other)
    {
        // キャラクターがトリガーゾーンに入ったかを確認
        if (canWarp && other.CompareTag("Player"))
        {
            StartCoroutine(CheckPlayerMoving(other));
        }
    }

    IEnumerator CheckPlayerMoving(Collider2D playerCollider)
    {
        PlayerController playerController = playerCollider.GetComponent<PlayerController>();
        while (playerController.isMoving)
        {
            yield return null; // 次のフレームまで待機
        }

        // シーン変更イベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;

        // シーンを変更
        SceneManager.LoadScene(targetScene);
    }

    // シーンがロードされた後に呼び出される関数
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Playerオブジェクトを探して位置を更新
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = spawnPosition;

            // プレイヤーの初期位置を更新
            playerInitialPosition = player.transform.position;

            // ワープ機能を再度無効にする
            canWarp = false;
        }

        // イベントからこの関数を削除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
