public class LayerData
{
    public int[,] Map { get; private set; }
    public string Name { get; private set; }

    public LayerData(string name, int[,] map)
    {
        Map = map;
        Name = name;
    }
}