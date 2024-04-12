
namespace Generate.Random
{
    using Random = System.Random;
    public class NumGenerator
    {
        static int
        //После какого количества чисел добавляется cycleValue
        cycle = 100,
        cycleValue = 15;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="seed"></param>
        /// <param name="mazeSizeX"></param>
        /// <returns></returns>
        public static int Gen(int x, int y, int seed, int mazeSizeX)
        {
            seed += Math.Abs(y * mazeSizeX + x) / cycle * cycleValue;
            Random r = new Random(seed);
            int num = r.Next();//Число, которое будет обновляться
            for (int i = 0; i < Math.Abs(y * mazeSizeX + x) % cycle; i++)
            {
                num = r.Next();
            }
            return num;
        }
    }
}