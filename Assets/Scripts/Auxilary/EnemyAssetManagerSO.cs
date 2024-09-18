using Assets.Scripts.Players.PlayerAI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Auxilary
{
    [CreateAssetMenu(fileName = "New Enemy Asset Manager", menuName = "Mau/New Enemy Asset Manager")]
    public  class EnemyAssetManagerSO : ScriptableObject
    {
        [SerializeField]
        UDictionary<AIStrategy, EnemyGenerationPoolSO> EnemyDecks;

        [SerializeField]
        UDictionary<AIEvalTypes, EnemyEvaStatsSO> EnemyValuationStats;

        public EnemyGenerationPoolSO GetEnemyPool(AIStrategy strategy)
        {
            if (strategy == AIStrategy.None)
            {
                return EnemyDecks.ElementAt(GameManager.currRun.RoundRand.NextInt(0, EnemyDecks.Count)).Value;
            }
            return EnemyDecks[strategy];
        }

        public EnemyGenerationPoolSO GetRandomEnemyDecs()
        {
            return EnemyDecks.Values[GameManager.currRun.RoundRand.NextInt(0, EnemyDecks.Count)];
        }

        public EnemyGenerationPoolSO[] GetRandomEnemyDecks(int amount)
        {
            return GameManager.currRun.RoundRand.GetRandomElements(EnemyDecks.Values, amount).ToArray();
        }

        public AIEvaluationValues getValues(AIEvalTypes evalType)
        {
            return EnemyValuationStats[evalType].stats;
        }

        public AIStrategy ReturnValidStrat()
        {
            return EnemyDecks.Keys[GameManager.currRun.RoundRand.NextInt(0, EnemyDecks.Keys.Count)];
        }
    }
}
