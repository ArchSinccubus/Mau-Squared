
using UnityEngine;


namespace Assets.Scripts.Players.PlayerAI
{
    [CreateAssetMenu(fileName = "New Enemy Type", menuName = "Mau/New Enemy Eval Stats")]
    internal class EnemyEvaStatsSO : ScriptableObject
    {
        public AIEvaluationValues stats;
    }
}
