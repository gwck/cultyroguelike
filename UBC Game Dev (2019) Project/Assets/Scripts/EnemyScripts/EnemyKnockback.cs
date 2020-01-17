using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    private EnemyStats stats;
    void Start()
    {
        stats = GetComponent<EnemyStats>();
    }
    private void OnCollisionEnter2D(Collision2D colliderInfo)
    {
        PlayerController _player = colliderInfo.collider.GetComponent<PlayerController>();

        if (_player != null)
        {
            _player.DamagePlayer(stats.enemyDamage);
            _player.forwardVelocity = _player.forwardVelocity - stats.enemyDamage;

            var _playerController = _player.GetComponent<PlayerController>();
            _playerController.knockbackCount = _playerController.knockbackLength;

            if (_player.transform.position.x < transform.position.x)
            {
                _playerController.knockFromRight = true;
            }
            else
            {
                _playerController.knockFromRight = false;
            }
        }
    }
}
