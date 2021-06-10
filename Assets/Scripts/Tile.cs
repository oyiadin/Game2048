using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            RefreshStyle();
        }
    }

    public bool Merged = false;
    
    public static readonly Dictionary<int, Color> BgColorMappings = new Dictionary<int, Color>
    {
        {2, new Color(0.9333f, 0.8941f, 0.8549f)},
        {4, new Color(0.9333f, 0.8823f, 0.7882f)},
        {8, new Color(0.9529f, 0.6980f, 0.4784f)},
        {16, new Color(0.9647f, 0.5882f, 0.3921f)},
        {32, new Color(0.9686f, 0.4862f, 0.3725f)},
        {64, new Color(0.9686f, 0.3725f, 0.2313f)},
        {128, new Color(0.9294f, 0.8156f, 0.4509f)},
        {256, new Color(0.9294f, 0.8000f, 0.3843f)},
        {512, new Color(0.9294f, 0.7882f, 0.3137f)},
        {1024, new Color(0.9294f, 0.7725f, 0.2470f)},
        {2048, new Color(0.9294f, 0.7607f, 0.1803f)},
    };

    public static readonly Dictionary<int, Color> ColorMappings = new Dictionary<int, Color>
    {
        {2, new Color(0.4666f, 0.4313f, 0.3960f)},
        {4, new Color(0.4666f, 0.4313f, 0.3960f)},
        {8, new Color(0.9764f, 0.9647f, 0.9490f)},
        {16, new Color(0.9764f, 0.9647f, 0.9490f)},
        {32, new Color(0.9764f, 0.9647f, 0.9490f)},
        {64, new Color(0.9764f, 0.9647f, 0.9490f)},
        {128, new Color(0.9764f, 0.9647f, 0.9490f)},
        {256, new Color(0.9764f, 0.9647f, 0.9490f)},
        {512, new Color(0.9764f, 0.9647f, 0.9490f)},
        {1024, new Color(0.9764f, 0.9647f, 0.9490f)},
        {2048, new Color(0.9764f, 0.9647f, 0.9490f)},
    };

    public static readonly Dictionary<int, int> FontSizeMappings = new Dictionary<int, int>
    {
        {2, 42},
        {4, 42},
        {8, 42},
        {16, 39},
        {32, 39},
        {64, 39},
        {128, 35},
        {256, 35},
        {512, 35},
        {1024, 30},
        {2048, 30},
    };

    private int _value;
    private Text _text;

    void Awake()
    {
        _text = GetComponentInChildren<Text>();
        Value = Random.Range(0, 10) < 9 ? 2 : 4;
    }

    private void RefreshStyle()
    {
        _text.text = _value.ToString();
        GetComponent<Image>().color = BgColorMappings[Value];
        _text.color = ColorMappings[Value];
        _text.fontSize = FontSizeMappings[Value];
    }
}
