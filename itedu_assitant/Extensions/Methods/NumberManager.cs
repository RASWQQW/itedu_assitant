namespace itedu_assitant.forsave.Methods
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
        public string GetNumberAsToken(object number)
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
    
    }
    public struct checkVal
    {

       public checkVal(string val1, string val2)
       {
            val11 = val1;
            val22 = val2;
       }
        public string val11 { get; private set; }
        public string val22 { get; private set; }

    }
}
