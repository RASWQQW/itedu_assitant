namespace itedu_assitant.forsave.Methods
{
    public class Manager : Object
    {

        public Manager(): base()
        {

        }

        public string GetNumberAsToken(object number)
        {
            string _modnumber = number.ToString();
            string retval = null;
            
            if (_modnumber.Length <= 11 && _modnumber.All(Char.IsDigit)){
                retval = (_modnumber.StartsWith("+") ? _modnumber[1..] : $"{Convert.ToInt32(_modnumber[0].ToString()) - 1}{_modnumber[1..]}")+"@c.us";
            }else{
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
