using System;
using System.Collections.Generic;

[Serializable]
public class MachineDTO
{
    public string id;       // "A","B","C"
    public string name;     // "Press A"
    public string status;   // "RUN","STOP","ALARM"
    public int output;      // 120,0,15
}

[Serializable]
public class LayoutDTO
{
    public List<MachineDTO> machines;
}
