using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;

    private void OnEnable()
    {
        PlayerController.OnPlayerDied += HandlePlayerDied;
    }
    
    private void OnDisable()
    {
        PlayerController.OnPlayerDied -= HandlePlayerDied;
    }

    private void Start()
    {
        players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
    }

    private void HandlePlayerDied(PlayerController deadPlayer)
    {
        foreach (var player in players)
        {
            if (player == null) continue;
            if (player == deadPlayer) continue;
            if (player.IsDead()) continue;

            OnPlayerWin(player);
            break;
        }
    }

    private void OnPlayerWin(PlayerController winner)
    {
        CameraZoomFocus cam = winner.GetComponentInChildren<CameraZoomFocus>();
        cam?.Focus();
        
        DisableLoserCameras(winner);

        winner.Celebrate();

        Time.timeScale = 0.6f;
    }
    
    private void DisableLoserCameras(PlayerController winner)
    {
        foreach (var player in players)
        {
            if (player == null || player == winner) continue;

            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
                Destroy(cam.gameObject);
            }
        }
    }
}
