using System;


namespace GeoRAT.Core.Commands
{


    [Serializable]
    public class Commands
    {
        public string CommandType { get; set; }
        public string CommandParams { get; set; }

        public Commands(string commandType, string commandParams)
        {
            CommandType = commandType;
            CommandParams = commandParams;
        }

        public Commands()
        {

        }

    }

}


