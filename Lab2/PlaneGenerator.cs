namespace Lab2
{
    internal class PlaneGenerator
    {
        private float step, sizeX, sizeY, startX, startY;

        public PlaneGenerator(Func<(float, float), float> funk, float step = 0.5f, float sizeX = 20, float sizeY = 20, float startX = 0, float startY = 0)
        {
            Funk = funk;
            this.step = step;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.startX = startX;
            this.startY = startY;
        }
        public Func<(float, float), float> Funk;

        float[]? vertex;

        public float[] Vertex
        {
            get
            {
                if (vertex == null)
                {
                    vertex = ReGenerateQuad();
                }
                return vertex;
            }
        }

        uint[]? indexes;

        public uint[] Indexes
        {
            get
            {
                if (indexes == null)
                {
                    indexes = ReGenerateIdexes();
                }
                return indexes;
            }
        }

        private float[] ReGenerateQuad()
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

        private uint[] ReGenerateIdexes()
        {
            int x = (int)(Math.Abs(sizeX - startX) / step);
            int y = (int)(Math.Abs(sizeY - startY) / step);

            int maxX = x;
            int maxY = y;

            List<uint> result = new List<uint>();

            int index = 0;
            bool jump = false;

            for (int i = 0; i < maxX-1; i++)
            {
                for (int j = 0; j < maxY+maxX-1; j++)
                {
                    result.Add((uint)index);
                    jump = !jump;
                    if (jump)
                    {
                        index += Math.Abs(y);
                        continue;
                    }
                    index -= Math.Abs(y-1);
                }
                jump = !jump;
                y = -y;
            }
            result.Add((uint)index);

            return result.ToArray();
        }
    }
}
