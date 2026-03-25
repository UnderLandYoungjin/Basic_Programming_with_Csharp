namespace array
{
    internal class Program
    {
        static void Main(string[] args)
        {

            int[] arr = { 64, 34, 25, 12, 22, 11, 90, 45 };
            Array.Sort(arr);

            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i]);
            }

        }
    }
}
