using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;


namespace Assets.Scripts.Auxilary
{
    public class CustomRandom
    {
        [JsonIgnore]
        private Random random;

        [JsonProperty]
        public int Seed;

        [JsonProperty]
        public int Counter;
        public CustomRandom(int seed)
        {
            Seed = seed;
            random = new Random(seed);
            Counter = 0;
        }

        [JsonConstructor]
        public CustomRandom(int Seed, int Counter)
        {
            this.Seed = Seed;
            SetCounter(Counter);
        }

        public int NextInt(int min, int max)
        {
            Counter++;

            int range = max - min - 1;
            double d = random.NextDouble();
            //Debug.Log(d);

            return min + (int)Math.Floor(d * range);
        }

        public float NextFloat(float min, float max)
        {
            Counter++;
            double mantissa = random.NextDouble();

            float final = Mathf.Lerp(min, max, (float)mantissa);

            return final;
        }

        public void SetCounter(int count)
        {
            Counter = count;
            random = new Random(Seed);
            for (int i = 0; i < Counter; i++)
            {
                random.NextDouble(); // Skip numbers
            }
        }

        public List<T> GetRandomElements<T>(List<T> list, int count)
        {
            int n = list.Count;

            // Perform a Fisher-Yates shuffle up to the Xth element
            for (int i = 0; i < count; i++)
            {
                // Select a random index from i to n-1
                int j = NextInt(i, n);

                // Swap element at i with element at j
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            // Return the first X elements
            return list.GetRange(0, count);
        }

        public List<T> GetRandomElements<T>(List<T> list)
        {
            int n = list.Count;

            // Perform a Fisher-Yates shuffle up to the Xth element
            for (int i = 0; i < n; i++)
            {
                // Select a random index from i to n-1
                int j = NextInt(i, n);

                // Swap element at i with element at j
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            // Return the first X elements
            return list;
        }
    }
}
