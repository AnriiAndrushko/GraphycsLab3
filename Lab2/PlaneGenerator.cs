namespace Lab2
{
    internal class PlaneGenerator
    {
        public Func<(float, float), float> Funk;

        float[]? quad;

        public float[] Quad
        {
            get
            {
                if (quad == null)
                {
                    quad = ReGenerateQuad();
                }
                return quad;
            }
        }
        public PlaneGenerator(Func<(float,float), float> funk)
        {
            Funk = funk;
        }
        public float[] ReGenerateQuad(float step = 0.1f, float sizeX = 20, float sizeY = 20, float startX = 0, float startY = 0)
        {
            var result = new List<float>();
            for (float x = startX; x < sizeX; x += step)
            {
                for (float y = startY; y < sizeY; y += step)
                {
                    result.Add(x);
                    result.Add(y);
                    result.Add(Funk((x,y)));
                }
            }
            return result.ToArray();
        }
    }
}
