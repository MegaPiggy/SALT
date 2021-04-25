using System;
using System.Collections.Generic;
using System.Linq;

public class Randoms
{
    public static Randoms SHARED = new Randoms();
    private readonly Random rand;
    private double nextNextGaussian;
    private bool haveNextNextGaussian;
    private object gaussLock = new object();

    public Randoms() => this.rand = new Random();

    public Randoms(int seed) => this.rand = new Random(seed);

    public int GetInt(int high) => this.NextInt(high);

    public int GetInt() => this.rand.Next();

    public int GetInRange(int low, int high) => low == high ? low : low + this.NextInt(high - low);

    public float GetFloat(float high) => (float)this.rand.NextDouble() * high;

    public float GetInRange(float low, float high) => (double)low == (double)high ? low : low + (float)(this.rand.NextDouble() * ((double)high - (double)low));

    public bool GetChance(int n) => this.NextInt(n) == 0;

    public bool GetProbability(float p) => this.rand.NextDouble() < (double)p;

    public bool GetBoolean() => this.NextInt(2) == 0;

    public float GetNormal(float mean, float dev) => (float)this.NextGaussian() * dev + mean;

    public T Pick<T>(T[] vals) => vals[this.GetInt(vals.Length)];

    public T Pick<T>(IEnumerator<T> iterator, T ifEmpty)
    {
        if (!iterator.MoveNext())
            return ifEmpty;
        T obj = iterator.Current;
        int n = 2;
        while ((object)iterator.Current != null && iterator.MoveNext())
        {
            T current = iterator.Current;
            if (this.NextInt(n) == 0)
                obj = current;
            ++n;
        }
        return obj;
    }

    public T Pick<T>(IEnumerable<T> enumerable, T ifEmpty) => this.Pick<T>(enumerable.GetEnumerator(), ifEmpty);

    public IEnumerable<T> Pick<T>(List<T> collection, int count)
    {
        List<int> options = Enumerable.Range(0, ((IEnumerable<T>)collection).Count<T>()).ToList<int>();
        for (int ii = 0; ii < count && options.Any<int>(); ++ii)
            yield return collection[this.Pluck<int>((ICollection<int>)options, 0)];
    }

    public IEnumerable<T> Pick<T>(
      List<T> collection,
      int count,
      Func<T, float> weightFunction)
    {
        List<int> options = Enumerable.Range(0, ((IEnumerable<T>)collection).Count<T>()).ToList<int>();
        Func<int, float> optionsWeightFunction = (Func<int, float>)(idx => weightFunction(collection[idx]));
        for (int ii = 0; ii < count && options.Any<int>(); ++ii)
        {
            int randomIndex = this.Pick<int>((ICollection<int>)options, optionsWeightFunction, -1);
            if (randomIndex == -1)
                break;
            yield return collection[randomIndex];
            options.Remove(randomIndex);
        }
    }

    public IEnumerable<T> Pick<T>(List<T> collection, int min, int max) => this.Pick<T>(collection, this.GetInRange(min, max));

    public T Pick<T>(ICollection<T> iterable, T ifEmpty) => this.PickPluck<T>(iterable, ifEmpty, false);

    public T Pick<T>(IDictionary<T, float> weightMap, T ifEmpty)
    {
        T obj = ifEmpty;
        float high = 0.0f;
        foreach (KeyValuePair<T, float> weight in (IEnumerable<KeyValuePair<T, float>>)weightMap)
        {
            float num = weight.Value;
            if ((double)num > 0.0)
            {
                high += num;
                if ((double)high == (double)num || (double)this.GetFloat(high) < (double)num)
                    obj = weight.Key;
            }
            else if ((double)num < 0.0)
                throw new ArgumentException(nameof(weightMap), "Weight less than 0: " + (object)weight);
        }
        return obj;
    }

    public T Pick<T>(ICollection<T> iterable, Func<T, float> weightFunction, T ifEmpty)
    {
        T obj1 = ifEmpty;
        float high = 0.0f;
        foreach (T obj2 in (IEnumerable<T>)iterable)
        {
            float num = weightFunction(obj2);
            if ((double)num > 0.0)
            {
                high += num;
                if ((double)high == (double)num || (double)this.GetFloat(high) < (double)num)
                    obj1 = obj2;
            }
            else if ((double)num < 0.0)
                throw new ArgumentException("weightMap", "Weight less than 0: " + (object)obj2);
        }
        return obj1;
    }

    public T Pluck<T>(ICollection<T> iterable, T ifEmpty) => this.PickPluck<T>(iterable, ifEmpty, true);

    protected T PickPluck<T>(ICollection<T> coll, T ifEmpty, bool remove)
    {
        int count = coll.Count;
        if (count == 0)
            return ifEmpty;
        if (coll is IList<T>)
        {
            IList<T> objList = (IList<T>)coll;
            int index = this.NextInt(count);
            T obj = objList[index];
            if (!remove)
                return obj;
            objList.RemoveAt(index);
            return obj;
        }
        IEnumerator<T> enumerator = coll.GetEnumerator();
        enumerator.MoveNext();
        for (int index = this.NextInt(count); index > 0; --index)
            enumerator.MoveNext();
        try
        {
            return enumerator.Current;
        }
        finally
        {
            if (remove)
                coll.Remove(enumerator.Current);
        }
    }

    protected static void Swap<T>(IList<T> list, int ii, int jj)
    {
        T obj = list[jj];
        list[jj] = list[ii];
        list[ii] = obj;
    }

    protected static void Swap<T>(T[] array, int ii, int jj)
    {
        T obj = array[ii];
        array[ii] = array[jj];
        array[jj] = obj;
    }

    private int NextInt(int n)
    {
        if (n <= 0)
            throw new ArgumentOutOfRangeException(nameof(n), "must be positive");
        int num1;
        int num2;
        do
        {
            num1 = this.rand.Next();
            num2 = num1 % n;
        }
        while (num1 - num2 + (n - 1) < 0);
        return num2;
    }

    private double NextGaussian()
    {
        lock (this.gaussLock)
        {
            if (this.haveNextNextGaussian)
            {
                this.haveNextNextGaussian = false;
                return this.nextNextGaussian;
            }
            double num1;
            double num2;
            double d;
            do
            {
                num1 = 2.0 * this.rand.NextDouble() - 1.0;
                num2 = 2.0 * this.rand.NextDouble() - 1.0;
                d = num1 * num1 + num2 * num2;
            }
            while (d >= 1.0 || d == 0.0);
            double num3 = Math.Sqrt(-2.0 * Math.Log(d) / d);
            this.nextNextGaussian = num2 * num3;
            this.haveNextNextGaussian = true;
            return num1 * num3;
        }
    }
}
