using UnityEngine;
using AIManager_namespace;

public class PlayerManager : CharacterManager
{
    private void Start()
    {
        base.Initialized();
        m_sceneCharactersManager.AddPlayerToCharaterList(this.gameObject, CharacterType.Player);
    }

    public override void GiveDamage<T>(T opponentManager, Vector3 hitPosition)
    {
        Debug.Log("PlayerManager Give Damage");
    }

    public override void TakeDamage(int damage)
    {
        Debug.Log("Player is damaged by opponent...");
        m_Health -= damage;
    }
}
