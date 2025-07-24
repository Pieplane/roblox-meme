using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
    public float delay;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок упал, нужен респавн");
            Transform parent = other.transform;
            if (explosion == null) return;

            explosion.transform.position = parent.transform.position;
            explosion.transform.rotation = parent.transform.rotation;
            PlayerInput playerInput = parent.GetComponent<PlayerInput>();
            if(playerInput != null)
            {
                playerInput.enabled = false;
            }
            //parent.gameObject.SetActive(false);
            //GameManager.Instance.EnterID(CheckpointManager.Instance.GetLastCheckpointID());
            //if(parent.childCount > 0)
            //{
            //    Transform mesh = parent.GetChild(1);
            //    mesh.gameObject.SetActive(false);
            //    StartCoroutine(CallWithDelay(mesh.gameObject, delay));
            //}
            parent.gameObject.SetActive(false);
            StartCoroutine(CallWithDelay(parent.gameObject, delay));
            explosion.gameObject.SetActive(true);

            

            
        }
    }
    private IEnumerator CallWithDelay(GameObject player, float delay)
    {
        Debug.Log("Ожидаю респавн");
        yield return new WaitForSeconds(delay);

        // Переместить и включить игрока
        GameManager.Instance.EnterID(CheckpointManager.Instance.GetLastCheckpointID());

        // 👇 можно убрать, если включение уже происходит внутри StagePreparation
        player.SetActive(true);

        // Обнуляем буфер прыжка
        ThirdPersonController controller = player.GetComponent<ThirdPersonController>();
        if (controller != null)
        {
            controller.ResetJumpBuffer();
        }
    }
}
