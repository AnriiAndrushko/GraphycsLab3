namespace Lab2
{
    public static class Shapes
    {
        public static float[] GetTorusQuadsVert(int numc = 100, int numt = 100)
        {
            List<float> result = new List<float>();
            double TWOPI = 2 * Math.PI;
            for (int i = 0; i < numc; i++)
            {
                for (int j = 0; j <= numt; j++)
                {
                    for (int k = 1; k >= 0; k--)
                    {

                        float s = (float)((i + k) % numc + 0.5);
                        float t = j % numt;

                        float x = (float)((1 + 0.1 * Math.Cos(s * TWOPI / numc)) * Math.Cos(t * TWOPI / numt));
                        float y = (float)((1 + 0.1 * Math.Cos(s * TWOPI / numc)) * Math.Sin(t * TWOPI / numt));
                        float z = (float)(0.1 * Math.Sin(s * TWOPI / numc));
                        result.Add(x);
                        result.Add(y);
                        result.Add(z);
                    }
                }
            }
            return result.ToArray();
        }
    }
}
