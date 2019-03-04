
public class Gauge
{
    private int _value;
    public int min { get; set; }
    public int max { get; set; }

    public Gauge(int value, int vmin, int vmax)
    {
        _value = value;
        min = vmin;
        max = vmax;
    }

    public int get()
    {
        return (_value);
    }

    public void set(int value)
    {
        _value = value;
        if (_value < min) _value = min;
        if (_value > max) _value = max;
    }
}