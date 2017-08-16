using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public Texture2D Texture;
    public GameObject TilePrefab;
    public GameObject CollisionRectangle;
    public GameObject CollisionPolygon;
    public string MapResource;
    
    private int _width = 5;
    private int _height = 5;
    private LayerData[] _layers;
    private Sprite[] _sprites;
 
    void Start()
    {
        _sprites = Resources.LoadAll<Sprite>(Texture.name);
        LoadXml();
        DrawMap();
        Camera.main.transform.position = new Vector3(3, 6, -12);
    }

    public void LoadXml()
    {
        TextAsset asset = Resources.Load(MapResource) as TextAsset;
        if (asset != null)
        {
            XDocument xmlDoc = XDocument.Load(new StringReader(asset.text));
            var xMap = xmlDoc.Element("map");

            _height = (int) xMap.Attribute("height");
            _width = (int) xMap.Attribute("width");

            var elements = xmlDoc.Descendants("map").Elements();

            var layers = 0;
            var xElements = elements as XElement[] ?? elements.ToArray();
            foreach (var element in xElements)
            {
                if (element.Name == "layer")
                {
                    layers++;
                }
            }

            _layers = new LayerData[layers];

            var layerIdx = 0;
            foreach (var element in xElements)
            {
                if (element.Name == "layer")
                {
                    AddLayer(element, layerIdx);
                    layerIdx++;
                }
                else if (element.Name == "objectgroup")
                {
                    AddCollsionLayer(element);
                }
            }
        }
        else
        {
            Debug.LogError("Settings file not found");
        }
    }

    private void AddCollsionLayer(XElement element)
    {
        foreach (var colObject in element.Elements())
        {
            var column = (float) colObject.Attribute("x") / 64f;
            var row = _height - ((float) colObject.Attribute("y") / 64f) - 1;

            GameObject obj;
            if (colObject.Attribute("type") != null && colObject.Attribute("type").Value == "hill")
            {
                AddPolygonCollider(column, row, colObject);
            }
            else
            {
                AddRectangleCollider(colObject, row, column);
            }
        }
    }

    private GameObject AddRectangleCollider(XElement colObject, float row, float column)
    {
        GameObject obj;
        var width = (float) colObject.Attribute("width") / 64f;
        var height = (float) colObject.Attribute("height") / 64f;
        string objectName;
        if (colObject.Attribute("name") == null)
        {
            objectName = "Unknown";
        }
        else
        {
            objectName = colObject.Attribute("name").Value;
        }
      
        row = (row - (height / 2f)) - 0.5f;
        column = (column + (width / 2f)) - 0.5f;

        var pos = new Vector3(column, row + 1, 0);
        obj = Instantiate(CollisionRectangle, pos, Quaternion.identity, this.transform);
        obj.transform.localScale = new Vector3(width, height, 1);
        obj.name = "collsion object"  + objectName;
        return obj;
    }

    private GameObject AddPolygonCollider(float column, float row, XElement colObject)
    {
        GameObject obj;
        var pos = new Vector3(column - 0.5f, row - 0.5f, 0);
        obj = Instantiate(CollisionPolygon, pos, Quaternion.identity, this.transform);

        var polygon = colObject.Element("polygon");
        var pairs = polygon.Attribute("points").Value.Split(' ');

        var polyCollider = obj.GetComponent<PolygonCollider2D>();

        Vector2[] points = new Vector2[pairs.Length];
        var i = 0;
        foreach (var pair in pairs)
        {
            var pointsPar = pair.Split(',');

            float pointX = float.Parse(pointsPar[0]);
            float pointY = float.Parse(pointsPar[1]);

            pointX = pointX / 64;
            pointY = 1 - pointY / 64;
            points[i++] = new Vector2(pointX, pointY);
        }
        polyCollider.points = points;
        return obj;
    }

    public void AddLayer(XElement element, int layerIdx)
    {
        var map = new int[_width, _height];
        var rawData = Convert.FromBase64String((string) element.Element("data").Value);
        Stream data = new MemoryStream(rawData, false);
        using (var br = new BinaryReader(data))
        {
            for (var j = 0; j < _height; j++)
            {
                for (var i = 0; i < _width; i++)
                {
                    map[i, _height - 1 - (j)] = (int) br.ReadUInt32();
                }
            }
        }
        _layers[layerIdx] = new LayerData(element.Attribute("name").ToString(), map);
    }

    private void DrawMap()
    {
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                var pos = new Vector3(x, y, 0);

                var tileId = _layers[0].Map[x, y];
                if (tileId == 0)
                {
                    continue;
                }

                var tileObj = Instantiate(TilePrefab, pos, Quaternion.identity, this.transform);
                tileObj.GetComponent<SpriteRenderer>().sprite = _sprites[tileId - 1];
            }
        }
    }
}