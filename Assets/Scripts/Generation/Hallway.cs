using UnityEngine;

public class Hallway : Connectable
{
    public Connectable? origin;
    public Connectable? end;

    public Vector3 originDoorPosition;
    public Vector3 endDoorPosition;
    
    public Connectable? Origin
    {
        get { return origin; }
        set { origin = value; }
    }

    public Connectable? End
    {
        get { return end; }
        set { end = value; }
    }

    public Hallway()
    {
        Connections = new[] { Origin, End };
    }

    public override string ToString()
    {
        return "H";
    }
}