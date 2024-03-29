﻿namespace itedu_assitant.forsave.Methods
{
    public class NumberManager : Object
    {

        public NumberManager(): base()
        {

        }

        public void GetProperNumber(string isnumber, out string outnumber)
        {

            // there number have to be either with + (+77789197489) number or default (87789197489)
            outnumber = isnumber;
            if (isnumber.Length <= 12)
            {
                outnumber = (isnumber.StartsWith("+")
                    ? isnumber[1..]
                    : $"{Convert.ToInt32(isnumber[0].ToString()) - 1}{isnumber[1..]}");
            }
        }
        public string GetNumberAsId(object number)
        {
            string _modnumber = number.ToString();
            string retval = null;
            
            if (_modnumber.Length <= 12){
                GetProperNumber(_modnumber, out retval);
                retval = retval + "@c.us";
            }
            else{

                // it work with group id if its too origin or to modify
                if (_modnumber.Length > 11 && _modnumber.Length <= 35)
                    if (_modnumber.Contains("@g.us"))
                        retval = _modnumber;
                    else 
                        retval = _modnumber+"@g.us";
            }

            return retval;

        }

        public string GetNumbreFromId(string WhatsapId)
        {
            if(WhatsapId.Contains("@c.us") && WhatsapId.Length <= 16)
                return (Convert.ToInt32(WhatsapId[0].ToString()) + 1).ToString() + WhatsapId[1..-5];
           
            return default;
        }
    
    }
}
