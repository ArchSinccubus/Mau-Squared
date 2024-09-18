using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using Assets.Scripts.Players.PlayerAI;


public class AIJsonConverter : JsonConverter<IAIBase>
{
    public override IAIBase ReadJson(JsonReader reader, Type objectType, IAIBase AI, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject j = JObject.Load(reader);

        AIType type = 0;
        AIStrategy strat1 = 0;
        AIStrategy strat2 = 0;
        Colors color = 0;
        AIEvaluationValues vals = new AIEvaluationValues();

        foreach (var item in j)
        {
            switch (item.Key)
            {
                case "AIType":
                    type = item.Value.ToObject<AIType>();
                    break;
                case "preferredStrategy":
                    strat1 = item.Value.ToObject<AIStrategy>();
                    break;
                case "secondaryStrategy":
                    strat2 = item.Value.ToObject<AIStrategy>();
                    break;
                case "preferredColor":
                    color = item.Value.ToObject<Colors>();
                    break;
                case "AIEvaluationValues":
                    vals = item.Value.ToObject<AIEvaluationValues>();
                    break;
                default:
                    break;
            }
        }

        IAIBase returnedType = new RandomAI();

        

        switch (type)
        {
            case AIType.Random:
                returnedType = new RandomAI();
                break;
            case AIType.Dumb:
                returnedType = new DumbAI();
                break;
            case AIType.Average:
                returnedType = new AverageAI();
                break;
            case AIType.Smart:
            case AIType.Master:
                returnedType = new SmartAI() { secondaryStrategy = strat2 };
                break;
            default:
                break;
        }

        returnedType.AIType = type;
        returnedType.preferredColor = color;
        returnedType.preferredStrategy = strat1;
        returnedType.AIEvaluationValues = vals;

        return returnedType;
    }

    public override void WriteJson(JsonWriter writer, IAIBase value, JsonSerializer serializer)
    {
        JToken t = JToken.FromObject(value);

        writer.WriteRawValue(JsonConvert.SerializeObject(value));
    }
}