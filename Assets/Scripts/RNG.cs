using System;
using System.Collections.Generic;

public static class RNG
{
    private static Random RANDOM = new Random();

    public static float Range(float min, float max)
    {
        float n = NextSingle();
        return min + n * (max - min);
    }

    public static int Range(int min, int max)
    {
        float n = NextSingle();
        max--;
        return (int)(min + n * (max - min));
    }

    public static int Next() => RANDOM.Next();
    public static float NextSingle() => (float)RANDOM.NextDouble();

    public static void SetSeed(int seed)
    {
        RANDOM = new(seed);
    }

    public static float GetTotalWeight<T>(IList<T> values) where T : IWeighted
    {
        float w = 0;
        for (int i = 0; i < values.Count; i++)
        {
            w += values[i].Weight;
        }
        return w;
    }

    public static T SelectRandom<T>(IList<WeightedType<T>> values, T defaultValue) => SelectRandom(values, values.Count, GetTotalWeight(values), defaultValue);
    public static T SelectRandom<T>(IList<WeightedType<T>> values, int count, T defaultValue) => SelectRandom(values, count, GetTotalWeight(values), defaultValue);

    public static T SelectRandom<T>(IList<WeightedType<T>> values, float totalWeight, T defaultValue) => SelectRandom(values, values.Count, totalWeight, defaultValue);
    public static T SelectRandom<T>(IList<WeightedType<T>> values, int count, float totalWeight, T defaultValue) 
    {
        var ind = SelectRandomIndex(values, count, totalWeight);
        return ind < 0 ? defaultValue : values[ind].value;
    }

    public static T SelectRandom<T>(IList<T> values, T defaultValue) where T : IWeighted => SelectRandom(values, values.Count, GetTotalWeight(values), defaultValue);
    public static T SelectRandom<T>(IList<T> values, int count, T defaultValue) where T : IWeighted => SelectRandom(values, count, GetTotalWeight(values), defaultValue);

    public static T SelectRandom<T>(IList<T> values, float totalWeight, T defaultValue) where T : IWeighted => SelectRandom(values, values.Count, totalWeight, defaultValue);
    public static T SelectRandom<T>(IList<T> values, int count, float totalWeight, T defaultValue) where T : IWeighted
    {
        var ind = SelectRandomIndex(values, count, totalWeight);
        return ind < 0 ? defaultValue : values[ind];
    }

    public static int SelectRandomIndex<T>(IList<T> values) where T : IWeighted => SelectRandomIndex(values, values.Count, GetTotalWeight(values));
    public static int SelectRandomIndex<T>(IList<T> values, int count) where T : IWeighted => SelectRandomIndex(values, count, GetTotalWeight(values));

    public static int SelectRandomIndex<T>(IList<T> values, float totalWeight) where T : IWeighted => SelectRandomIndex(values, values.Count, totalWeight);
    public static int SelectRandomIndex<T>(IList<T> values, int count, float totalWeight) where T : IWeighted
    {
        float rnd = NextSingle() * totalWeight;

        float prevAcc;
        float currAcc = 0;
        int selected = 0;
        float select = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            prevAcc = currAcc;
            currAcc += values[i].Weight;

            if (rnd <= currAcc && rnd >= prevAcc)
            {
                float d = Math.Abs(currAcc - rnd);
                if(d < select)
                {
                    select = d;
                    selected = i;
                }
            }
        }
        return selected;
    }
}